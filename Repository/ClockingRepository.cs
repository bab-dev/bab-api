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
    public class ClockingRepository : RepositoryBase<Clocking>, IClockingRepository
    {
        private readonly ISortHelper<Clocking> _sortHelper;
        public ClockingRepository(RepositoryContext repositoryContext, ISortHelper<Clocking> sortHelper)
            : base(repositoryContext)
        {
            _sortHelper = sortHelper;
        }

        public PagedList<Clocking> GetAllClockings([Optional] ClockingParameters parameters)
        {
            IQueryable<Clocking> clockingRegistries= FindByCondition(clocking =>
                   ((clocking.IDVolunteer == parameters.IDVolunteer || parameters.IDVolunteer == null)
                       && (clocking.Date.Month == parameters.Month || parameters.Month == null)
                    && (clocking.Date.Year == parameters.Year || parameters.Year == null)));

            if (parameters is not null)
            {
                var sortedClockingRegistries = _sortHelper.ApplySort(clockingRegistries, parameters.OrderBy);

                if (parameters.PageSize.HasValue)
                {
                    return PagedList<Clocking>.ToPagedList(sortedClockingRegistries, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<Clocking>.ToPagedList(sortedClockingRegistries, 1, clockingRegistries.Count());
                }
            }
            else
            {
                var sortedClockingRegistries = _sortHelper.ApplySort(clockingRegistries, "Date desc");
                return PagedList<Clocking>.ToPagedList(sortedClockingRegistries, 1, clockingRegistries.Count());
            }            
        }

        public Clocking GetClockingById(Guid idClocking)
        {
            return FindByCondition(clocking => clocking.Id.Equals(idClocking))
                    .FirstOrDefault();
        }

        public Clocking GetClockingByIdVolunteerAndDate(int idVolunteer, DateTime date)
        {
            return FindByCondition(clocking => clocking.IDVolunteer.Equals(idVolunteer) && clocking.Date.Equals(date))
                    .FirstOrDefault();
        }

        public void CreateClocking(Clocking clocking)
        {
            clocking.Id = Guid.NewGuid();
            Create(clocking);
            Save();
        }

        public void UpdateClocking(Clocking clocking)
        {
            Update(clocking);
            Save();
        }

        public void DeleteClocking(Clocking clocking)
        {
            Delete(clocking);
            Save();
        }
    }
}
