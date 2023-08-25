using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        private readonly ISortHelper<Event> _sortHelper;
        public EventRepository(RepositoryContext repositoryContext, ISortHelper<Event> sortHelper)
            : base(repositoryContext)
        {
            _sortHelper = sortHelper;
        }

        private void SearchByTitle(ref IQueryable<Event> events, string title)
        {
            if (!events.Any() || string.IsNullOrWhiteSpace(title))
                return;

            events = events.Where(e => e.Title.ToLower().Contains(title.Trim().ToLower()));
        }

        public PagedList<Event> GetAllEvents([Optional] EventParameters parameters)
        {
            if (parameters is not null)
            {
                var events = FindByCondition(e =>
                (e.IDDepartment == parameters.IDDepartment || parameters.IDDepartment == null));

                SearchByTitle(ref events, parameters.Title);
                var sortedEvents = _sortHelper.ApplySort(events, parameters.OrderBy);
                if (parameters.PageSize.HasValue)
                {
                    return PagedList<Event>.ToPagedList(sortedEvents, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<Event>.ToPagedList(sortedEvents, 1, sortedEvents.Count());
                }
            } else
            {
                var events = FindAll();
                var sortedEvents = _sortHelper.ApplySort(events, "Start");
                return PagedList<Event>.ToPagedList(sortedEvents, 1, sortedEvents.Count());
            }
        }

        public Event GetEventById(Guid idEvent)
        {
            return FindByCondition(eventRetrieved => eventRetrieved.Id.Equals(idEvent))
                    .FirstOrDefault();
        }

        public void CreateEvent(Event eventToCreate)
        {
            eventToCreate.Id = Guid.NewGuid();
            Create(eventToCreate);
            Save();
        }

        public void UpdateEvent(Event eventToUpdate)
        {
            Update(eventToUpdate);
            Save();
        }

        public void DeleteEvent(Event eventToDelete)
        {
            Delete(eventToDelete);
            Save();
        }
    }
}
