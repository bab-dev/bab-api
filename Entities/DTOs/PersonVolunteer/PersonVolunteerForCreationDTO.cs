using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PersonVolunteerForCreationDTO
    {
        public PersonForCreationAndUpdateDTO Person { get; set; }
        public NewVolunteerForCreationDTO Volunteer { get; set; }
        public NewVolunteerAvailabilityForCreationDTO VolunteerAvailability { get; set; }
        public NewEmergencyContactForCreation EmergencyContact { get; set; }
    }
}
