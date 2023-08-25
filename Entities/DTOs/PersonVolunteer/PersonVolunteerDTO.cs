using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class PersonVolunteerDTO
    {
        public PersonDTO Person { get; set; }
        public VolunteerDTO Volunteer { get; set; }
        public VolunteerAvailabilityDTO VolunteerAvailability { get; set; }
        public EmergencyContactDTO EmergencyContact { get; set; }
    }
}
