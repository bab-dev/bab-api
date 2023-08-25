using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VolunteerFromEventDTO
    {
        public int IDVolunteer { get; set; }
        public Guid IDEvent { get; set; }
        public string FullName { get; set; }

    }
}
