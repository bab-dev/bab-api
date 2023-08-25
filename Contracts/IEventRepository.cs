using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEventRepository : IRepositoryBase<Event>
    {
        PagedList<Event> GetAllEvents([Optional] EventParameters eventParameters);
        Event GetEventById(Guid idEvent);
        void CreateEvent(Event newEvent);
        void UpdateEvent(Event eventToUpdate);
        void DeleteEvent(Event eventToDelete);
    }
}
