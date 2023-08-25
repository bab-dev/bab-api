using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EventVolunteerRepository : RepositoryBase<EventVolunteer>, IEventVolunteerRepository
    {
        private readonly IEventRepository _eventRepository;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly ISortHelper<Event> _sortHelper;

        public EventVolunteerRepository(RepositoryContext repositoryContext, IEventRepository eventRepository, IVolunteerRepository volunteerRepository, ISortHelper<Event> sortHelper)
            : base(repositoryContext)
        {
            _eventRepository = eventRepository;
            _volunteerRepository = volunteerRepository;
            _sortHelper = sortHelper;
        }
        public IEnumerable<EventVolunteer> GetAllEventVolunteers()
        {
            return FindAll()
                .OrderBy(eventVolunteerRetrieved => eventVolunteerRetrieved.Id)
                .ToList();
        }

        public EventVolunteer GetEventVolunteerById(Guid idEventVolunteer)
        {
            return FindByCondition(eventVolunteerRetrieved => eventVolunteerRetrieved.Id.Equals(idEventVolunteer))
                    .FirstOrDefault();
        }

        public EventVolunteer GetEventVolunteerByIDEventAndIDVolunteer(Guid idEvent, int idVolunteer)
        {
            return FindByCondition(eventVolunteerRetrieved => eventVolunteerRetrieved.IDEvent.Equals(idEvent) && eventVolunteerRetrieved.IDVolunteer.Equals(idVolunteer)).FirstOrDefault();
        }

        private void SearchByTitle(ref IQueryable<Event> events, string title)
        {
            if (!events.Any() || string.IsNullOrWhiteSpace(title))
                return;

            events = events.Where(e => e.Title.ToLower().Contains(title.Trim().ToLower()));
        }

        public PagedList<Event> GetEventsByIdVolunteer(int idVolunteer, [Optional] EventParameters eventParameters)
        {
            var eventIds = FindAll()
                    .Where(eventVolunteer => eventVolunteer.IDVolunteer.Equals(idVolunteer))
                    .Select(eventVolunteer => eventVolunteer.IDEvent)
                    .ToList();

            if (_eventRepository == null)
            {
                throw new ArgumentNullException(nameof(_eventRepository), "Event repository is not initialized.");
            }

            var allEvents = _eventRepository.GetAllEvents(null);

            if (allEvents == null)
            {
                throw new Exception("Unable to retrieve events from the event repository.");
            }

            var volunteerEvents =  allEvents
                .Where(e => eventIds.Contains(e.Id))
                .OrderByDescending(eventRetrieved => eventRetrieved.Start)
                .ToList();

            var queryableVolunteerEvents = volunteerEvents.AsQueryable();

            // Convert the filtered events back to a PagedList
            if (eventParameters is not null)
            {
                var filteredEvents = volunteerEvents.Where(e => e.IDDepartment == eventParameters.IDDepartment || eventParameters.IDDepartment == null)
                    .OrderByDescending(eventRetrieved => eventRetrieved.Start)
                    .ToList();

                var queryableFilteredVolunteerEvents = filteredEvents.AsQueryable();

                SearchByTitle(ref queryableFilteredVolunteerEvents, eventParameters.Title);
                var sortedEvents = _sortHelper.ApplySort(queryableFilteredVolunteerEvents, eventParameters.OrderBy);

                if (eventParameters.PageSize.HasValue)
                {
                    return PagedList<Event>.ToPagedList(sortedEvents, eventParameters.PageNumber, eventParameters.PageSize.Value);
                }
                else
                {
                    return PagedList<Event>.ToPagedList(sortedEvents, eventParameters.PageNumber, queryableFilteredVolunteerEvents.Count());
                }
            }
            else
            {
                return PagedList<Event>.ToPagedList(queryableVolunteerEvents, 1, queryableVolunteerEvents.Count());
            }
        }

        public IEnumerable<Event> GetEventsByIdVolunteerAndDate(int idVolunteer, DateTime date)
        {
            var eventIds = FindAll()
                    .Where(eventVolunteer => eventVolunteer.IDVolunteer.Equals(idVolunteer))
                    .Select(eventVolunteer => eventVolunteer.IDEvent)
                    .ToList();

            if (_eventRepository == null)
            {
                throw new ArgumentNullException(nameof(_eventRepository), "Event repository is not initialized.");
            }

            var allEvents = _eventRepository.GetAllEvents(null);

            if (allEvents == null)
            {
                throw new Exception("Unable to retrieve events from the event repository.");
            }

            var volunteerEvents = allEvents
                .Where(e => eventIds.Contains(e.Id) && e.Start.Date.Equals(date))
                .OrderByDescending(eventRetrieved => eventRetrieved.Start)
                .ToList();

            return volunteerEvents;
        }

        public IEnumerable<Volunteer> GetVolunteersByIdEvent(Guid idEvent, [Optional] VolunteerParameters volunteerParameters)
        {
            var volunteerIds = FindAll()
                    .Where(eventVolunteer => eventVolunteer.IDEvent.Equals(idEvent))
                    .Select(eventVolunteer => eventVolunteer.IDVolunteer)
                    .ToList();

            if (_volunteerRepository == null)
            {
                throw new ArgumentNullException(nameof(_eventRepository), "Volunteer repository is not initialized.");
            }

            var allVolunteers = _volunteerRepository.GetAllVolunteers(volunteerParameters);

            if (allVolunteers == null)
            {
                throw new Exception("Unable to retrieve volunteers from the volunteer repository.");
            }
            return allVolunteers.Where(volunteer => volunteerIds.Contains(volunteer.Id)).OrderByDescending(volunteer => volunteer.Id).ToList();
        }

        public void CreateEventVolunteer(EventVolunteer eventVolunteerToCreate)
        {
            if (_eventRepository == null)
            {
                throw new ArgumentNullException(nameof(_eventRepository), "Event repository is not initialized.");
            }

            var selectedEvent = _eventRepository.GetEventById(eventVolunteerToCreate.IDEvent);
            if(selectedEvent is null)
            {
                throw new Exception("Selected event hasn't been found in database.");
            }

            if(selectedEvent.NumberOfVolunteersAssigned < selectedEvent.NumberOfVolunteersRequired)
            {
                //Update NumberOfVolunteersAssigned attribute on Event table
                selectedEvent.NumberOfVolunteersAssigned++;
                _eventRepository.UpdateEvent(selectedEvent);

                eventVolunteerToCreate.Id = Guid.NewGuid();
                Create(eventVolunteerToCreate);
                Save();
            } else
            {
                throw new Exception("This event already has all necessary volunteers assgined.");
            }
        }

        public void DeleteEventVolunteer(EventVolunteer eventVolunteerToDelete)
        {
            var selectedEvent = _eventRepository.GetEventById(eventVolunteerToDelete.IDEvent);
            if (selectedEvent is null)
            {
                throw new Exception("Selected event hasn't been found in database.");
            }
            //Update NumberOfVolunteersAssigned attribute on Event table
            selectedEvent.NumberOfVolunteersAssigned--;
            _eventRepository.UpdateEvent(selectedEvent);

            Delete(eventVolunteerToDelete);
            Save();
        }
    }
}
