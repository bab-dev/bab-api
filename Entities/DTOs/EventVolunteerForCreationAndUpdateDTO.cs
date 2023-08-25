using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EventVolunteerForCreationAndUpdateDTO
    {
        public Guid IDEvent { get; set; }
        public int IDVolunteer { get; set; }
    }
}
