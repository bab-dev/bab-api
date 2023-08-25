using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("emergency_contact")]
    public class EmergencyContact
    {
        [Key]
        [Column("IDEmergencyContact")]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Volunteer))]
        public int IDVolunteer { get; set; }

        [Required(ErrorMessage = "Emergency contact's name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Emergency contact's last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Emergency contact's phone number is required")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "Emergency contact's relationship is required")]
        public EmergencyContactRelationship Relationship { get; set; }
    }
}
