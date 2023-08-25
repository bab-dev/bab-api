using Contracts;
using Entities;
using Entities.DTOs.Clocking;
using Entities.Helpers;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class VolunteerEventWorkedOnRepository : RepositoryBase<VolunteerEventWorkedOn>, IVolunteerEventWorkedOnRepository
    {
        private readonly IClockingRepository _clockingRepository;
        private readonly ISortHelper<VolunteerWorkStatisticsDTO> _sortHelper;
        public VolunteerEventWorkedOnRepository(RepositoryContext repositoryContext, ISortHelper<VolunteerWorkStatisticsDTO> sortHelper, IClockingRepository clocking)
           : base(repositoryContext)
        {
            _clockingRepository = clocking;
            _sortHelper = sortHelper;
        }

        public IEnumerable<VolunteerEventWorkedOn> GetAllVolunteerEventWorkedOns()
        {
            return FindAll()
                 .OrderByDescending(eventWorkedOn => eventWorkedOn.Date)
                 .ToList();
        }

        public VolunteerEventWorkedOn GetVolunteerEventWorkedOnById(Guid idVolunteerEventWorkedOn)
        {
            return FindByCondition(eventWorkedOn => eventWorkedOn.Id.Equals(idVolunteerEventWorkedOn))
                    .FirstOrDefault();
        }

        //Gets the percentage of the times that the volunteer worked on an specific Event Type
        private int GetPercentage(Dictionary<int, int> eventTypeCounts, int eventType, int totalEvents)
        {
            if (eventTypeCounts.ContainsKey(eventType))
            {
                var count = eventTypeCounts[eventType];
                var percentage = (double)count / totalEvents * 100;
                return (int)Math.Round(percentage);
            }

            return 0; // If there's no record of this eventType, return 0
        }

        public VolunteerWorkStatisticsDTO GetVolunteerWorkStatisticsByIdVolunteer(int idVolunteer)
        {
            // Get all clocking entries of the volunteer
            var clockings = _clockingRepository.FindAll().Where(c => c.IDVolunteer == idVolunteer).ToList();

            // Calculate total hours worked
            var totalHoursWorked = TimeSpan.Zero;
            foreach (var clocking in clockings)
            {
                var hoursWorked = TimeSpan.ParseExact(clocking.TotalHoursWorked, @"hh\:mm", CultureInfo.InvariantCulture);
                totalHoursWorked += hoursWorked;
            }

            // Get all event entries off the volunteer
            var volunteerEvents = FindAll().Where(e => e.IDVolunteer == idVolunteer).ToList();

            // Calculate event type percentages
            var eventTypeCounts = volunteerEvents.GroupBy(e => e.EventType)
                .ToDictionary(g => g.Key, g => g.Count());

            int totalEvents = volunteerEvents.Count;

            // Create DTO
            var statistics = new VolunteerWorkStatisticsDTO
            {
                Id = Guid.NewGuid(), // or some suitable ID
                IDVolunteer = idVolunteer,
                TotalHoursWorked = totalHoursWorked.ToString(@"hh\:mm"), // Format the TimeSpan to a string
                PercenatgeWorkedOnDelivery = GetPercentage(eventTypeCounts, 0, totalEvents),
                PercenatgeWorkedOnPickUp = GetPercentage(eventTypeCounts, 1, totalEvents),
                PercenatgeWorkedOnFoodSelection = GetPercentage(eventTypeCounts, 2, totalEvents),
                PercenatgeWorkedOnDistributionToFamilies = GetPercentage(eventTypeCounts, 3, totalEvents),
                PercenatgeWorkedOnCleaning = GetPercentage(eventTypeCounts, 4, totalEvents),
                PercenatgeWorkedOnMeetings = GetPercentage(eventTypeCounts, 5, totalEvents),
                PercenatgeWorkedOnOtherTasks = GetPercentage(eventTypeCounts, 6, totalEvents),
            };

            return statistics;
        }

        public PagedList<VolunteerWorkStatisticsDTO> GetAllVolunteerWorkStatistics([Optional] VolunteerWorkStatisticsParameters parameters)
        {
            var allStatistics = new List<VolunteerWorkStatisticsDTO>();
            var allVolunteerIds = FindAll().Select(entry => entry.IDVolunteer).Distinct().ToList();

            if (parameters is not null)
            {
                if(parameters.IDVolunteer is not null)
                {
                    if (allVolunteerIds.Contains((int)parameters.IDVolunteer))
                    {
                        var volunteerStatistics = GetVolunteerWorkStatisticsByIdVolunteer((int)parameters.IDVolunteer);
                        if (volunteerStatistics is not null)
                            allStatistics.Add(volunteerStatistics);
                    }
                    
                } 
                else
                {
                    foreach (var idVolunteer in allVolunteerIds)
                    {
                        var volunteerStatistics = GetVolunteerWorkStatisticsByIdVolunteer(idVolunteer);
                        if (volunteerStatistics is not null)
                            allStatistics.Add(volunteerStatistics);
                    }
                }

                var sortedStadistics = _sortHelper.ApplySort(allStatistics.AsQueryable(), parameters.OrderBy);

                //Validate pagination params
                if (parameters.PageSize.HasValue)
                {
                    return PagedList<VolunteerWorkStatisticsDTO>.ToPagedList(sortedStadistics, parameters.PageNumber, parameters.PageSize.Value);
                }

                return PagedList<VolunteerWorkStatisticsDTO>.ToPagedList(sortedStadistics, parameters.PageNumber, allStatistics.Count);
            }
            var sortedStadisticsById = _sortHelper.ApplySort(allStatistics.AsQueryable(), "Id");
            return PagedList<VolunteerWorkStatisticsDTO>.ToPagedList(sortedStadisticsById, 1, allStatistics.Count);
        }

        public void CreateVolunteerEventWorkedOn(VolunteerEventWorkedOn eventWorkedOn)
        {
            eventWorkedOn.Id = Guid.NewGuid();
            Create(eventWorkedOn);
            Save();
        }

        public void UpdateVolunteerEventWorkedOn(VolunteerEventWorkedOn eventWorkedOn)
        {
            Update(eventWorkedOn);
            Save();
        }

        public void DeleteVolunteerEventWorkedOn(VolunteerEventWorkedOn eventWorkedOn)
        {
            Delete(eventWorkedOn);
            Save();
        }
    }
}
