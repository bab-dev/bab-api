﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Clocking
{
    public class ClockingForUpdateDTO
    {
        public int IDVolunteer { get; set; }
        public DateTime Date { get; set; }

        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$",
        ErrorMessage = "ClockInTime must be in the format HH:MM")]
        public string ClockInTime { get; set; }

        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$",
        ErrorMessage = "ClockOutTime must be in the format HH:MM")]
        public string? ClockOutTime { get; set; }
    }
}
