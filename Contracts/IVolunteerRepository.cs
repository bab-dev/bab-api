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
    public interface IVolunteerRepository : IRepositoryBase<Volunteer>
    {
        PagedList<Volunteer> GetAllVolunteers([Optional] VolunteerParameters volunteerParameters);
        Volunteer GetVolunteerById(int idVolunteer);
        Volunteer GetVolunteerByPersonId(Guid idPerson);
        string GetVolunteerFullName(int idVolunteer);
        void CreateVolunteer(Volunteer volunteer);
        void UpdateVolunteer(Volunteer volunteer);
        void DeleteVolunteer(Volunteer volunteer);
    }
}
