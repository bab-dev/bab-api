using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("volunteer_clocking")]
    public class Clocking
    {
        [Key]
        [Column("IDVolunteerClocking")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "IDVolunteer is required")]
        [ForeignKey(nameof(Volunteer))]
        public int IDVolunteer { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Column(TypeName = "time")]
        [Required(ErrorMessage = "ClockInTime is required")]
        public TimeSpan ClockInTime { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan ClockOutTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string TotalHoursWorked { get; set; }
    }
}
