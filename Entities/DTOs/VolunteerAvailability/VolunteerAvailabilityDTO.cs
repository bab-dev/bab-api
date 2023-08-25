using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VolunteerAvailabilityDTO
    {
        public Guid Id { get; set; }
        public int IDVolunteer { get; set; }
        public bool IsAvailableMondayMorning { get; set; }
        public bool IsAvailableMondayEvening { get; set; }
        public bool IsAvailableMondayNight { get; set; }
        public bool IsAvailableTuesdayMorning { get; set; }
        public bool IsAvailableTuesdayEvening { get; set; }
        public bool IsAvailableTuesdayNight { get; set; }
        public bool IsAvailableWednesdayMorning { get; set; }
        public bool IsAvailableWednesdayEvening { get; set; }
        public bool IsAvailableWednesdayNight { get; set; }
        public bool IsAvailableThrusdayMorning { get; set; }
        public bool IsAvailableThrusdayEvening { get; set; }
        public bool IsAvailableThrusdayNight { get; set; }
        public bool IsAvailableFridayMorning { get; set; }
        public bool IsAvailableFridayEvening { get; set; }
        public bool IsAvailableFridayNight { get; set; }
        public bool IsAvailableSaturdayMorning { get; set; }
        public bool IsAvailableSaturdayEvening { get; set; }
        public bool IsAvailableSaturdayNight { get; set; }
        public bool IsAvailableSundayMorning { get; set; }
        public bool IsAvailableSundayEvening { get; set; }
        public bool IsAvailableSundayNight { get; set; }
    }
}
