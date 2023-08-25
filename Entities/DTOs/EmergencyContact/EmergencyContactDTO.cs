using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EmergencyContactDTO
    {
        public Guid Id { get; set; }
        public int IDVolunteer { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public int Relationship { get; set; }
    }
}
