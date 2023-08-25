using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVolunteerAvailabilityRepository
    {
        IEnumerable<VolunteerAvailability> GetAllVolunteerAvailabilities();
        VolunteerAvailability GetAvailabilityByVolunteerId(int idVolunteer);
        void CreateVolunteerAvailability(VolunteerAvailability volunteerAvailability);
        void UpdateVolunteerAvailability(VolunteerAvailability volunteerAvailability);
        void DeleteVolunteerAvailability(VolunteerAvailability volunteerAvailability);
    }
}
