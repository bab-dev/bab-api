using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("volunteer_event_worked_on")]
    public class VolunteerEventWorkedOn
    {
        [Key]
        [Column("IDVolunteerEventWorkedOn")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "IDVolunteerClocking is required")]
        [ForeignKey(nameof(Clocking))]
        public Guid IDVolunteerClocking { get; set; }

        [Required(ErrorMessage = "IDVolunteer is required")]
        public int IDVolunteer { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "EventType is required")]
        public int EventType { get; set; }
    }
}
