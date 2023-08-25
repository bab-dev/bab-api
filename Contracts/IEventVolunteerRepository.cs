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
    public interface IEventVolunteerRepository : IRepositoryBase<EventVolunteer>
    {
        IEnumerable<EventVolunteer> GetAllEventVolunteers();
        EventVolunteer GetEventVolunteerById(Guid idEventVolunteer);
        EventVolunteer GetEventVolunteerByIDEventAndIDVolunteer(Guid idEvent, int idVolunteer);
        PagedList<Event> GetEventsByIdVolunteer(int idVolunteer, [Optional] EventParameters eventParameters);
        IEnumerable<Event> GetEventsByIdVolunteerAndDate(int idVolunteer, DateTime date);
        IEnumerable<Volunteer> GetVolunteersByIdEvent(Guid idEvent, [Optional] VolunteerParameters volunteerParameters);
        void CreateEventVolunteer(EventVolunteer newEventVolunteer);
        void DeleteEventVolunteer(EventVolunteer eventVolunteerToDelete);
    }
}
