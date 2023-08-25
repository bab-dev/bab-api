using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Clocking
{
    public class VolunteerWorkStatisticsDTO
    {
        public Guid Id { get; set; }
        public int IDVolunteer { get; set; }
        public string TotalHoursWorked { get; set; }
        public int PercenatgeWorkedOnDelivery { get; set; }
        public int PercenatgeWorkedOnPickUp { get; set; }
        public int PercenatgeWorkedOnFoodSelection { get; set; }
        public int PercenatgeWorkedOnDistributionToFamilies { get; set; }
        public int PercenatgeWorkedOnCleaning { get; set; }
        public int PercenatgeWorkedOnMeetings { get; set; }
        public int PercenatgeWorkedOnOtherTasks { get; set; }
    }
}
