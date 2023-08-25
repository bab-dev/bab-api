using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("event_volunteer")]
    public class EventVolunteer
    {
        [Key]
        [Column("IDEventVolunteer")]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Event))]
        [Required(ErrorMessage = "IDEvent is required")]
        public Guid IDEvent { get; set; }

        [ForeignKey(nameof(Volunteer))]
        [Required(ErrorMessage = "IDVolounteer is required")]
        public int IDVolunteer { get; set; }

    }
}
