using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class NewVolunteerAvailabilityForCreationDTO
    {
        [Required(ErrorMessage = "Availability on Monday morning param is required")]
        public bool IsAvailableMondayMorning { get; set; }

        [Required(ErrorMessage = "Availability on Monday evening is required")]
        public bool IsAvailableMondayEvening { get; set; }

        [Required(ErrorMessage = "Availability on Monday night is required")]
        public bool IsAvailableMondayNight { get; set; }

        [Required(ErrorMessage = "Availability on Tuesday morning param is required")]
        public bool IsAvailableTuesdayMorning { get; set; }

        [Required(ErrorMessage = "Availability on Tuesday evening is required")]
        public bool IsAvailableTuesdayEvening { get; set; }

        [Required(ErrorMessage = "Availability on Tuesday night is required")]
        public bool IsAvailableTuesdayNight { get; set; }

        [Required(ErrorMessage = "Availability on Wednesday morning param is required")]
        public bool IsAvailableWednesdayMorning { get; set; }

        [Required(ErrorMessage = "Availability on Wednesday evening is required")]
        public bool IsAvailableWednesdayEvening { get; set; }

        [Required(ErrorMessage = "Availability on Wednesday night is required")]
        public bool IsAvailableWednesdayNight { get; set; }

        [Required(ErrorMessage = "Availability on Thrusday morning param is required")]
        public bool IsAvailableThrusdayMorning { get; set; }

        [Required(ErrorMessage = "Availability on Thrusday evening is required")]
        public bool IsAvailableThrusdayEvening { get; set; }

        [Required(ErrorMessage = "Availability on Thrusday night is required")]
        public bool IsAvailableThrusdayNight { get; set; }

        [Required(ErrorMessage = "Availability on Friday morning param is required")]
        public bool IsAvailableFridayMorning { get; set; }

        [Required(ErrorMessage = "Availability on Friday evening is required")]
        public bool IsAvailableFridayEvening { get; set; }

        [Required(ErrorMessage = "Availability on Friday night is required")]
        public bool IsAvailableFridayNight { get; set; }

        [Required(ErrorMessage = "Availability on Saturday morning param is required")]
        public bool IsAvailableSaturdayMorning { get; set; }

        [Required(ErrorMessage = "Availability on Saturday evening is required")]
        public bool IsAvailableSaturdayEvening { get; set; }

        [Required(ErrorMessage = "Availability on Saturday night is required")]
        public bool IsAvailableSaturdayNight { get; set; }

        [Required(ErrorMessage = "Availability on Sunday morning param is required")]
        public bool IsAvailableSundayMorning { get; set; }

        [Required(ErrorMessage = "Availability on Sunday evening is required")]
        public bool IsAvailableSundayEvening { get; set; }

        [Required(ErrorMessage = "Availability on Sunday night is required")]
        public bool IsAvailableSundayNight { get; set; }
    }
}
