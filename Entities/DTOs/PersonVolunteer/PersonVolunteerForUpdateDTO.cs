using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class PersonVolunteerForUpdateDTO
    {
        public PersonForCreationAndUpdateDTO Person { get; set; }
        public VolunteerForCreationAndUpdateDTO Volunteer { get; set; }
        public VolunteerAvailabilityForUpdateDTO VolunteerAvailability { get; set; }
        public EmergencyContactForUpdateDTO EmergencyContact { get; set; }
    }
}
