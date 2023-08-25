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
    [Table("event")]
    public class Event
    {
        [Key]
        [Column("IDEvent")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "EventName is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "EventDescription is required")]
        public string EventDescription { get; set; }

        [Required(ErrorMessage = "EventType is required")]
        public EventType EventType { get; set; }
        
        [ForeignKey(nameof(Department))]
        [Required(ErrorMessage = "IdDepartment is required")]
        public Guid IDDepartment{ get; set; }

        [Required(ErrorMessage = "StartDate is required")]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "EndDate is required")]
        public DateTime End { get; set; }
        public string Observations { get; set; }

        [Required(ErrorMessage = "NumberOfVolunteersRequired is required")]
        public int NumberOfVolunteersRequired { get; set; }

        public int NumberOfVolunteersAssigned { get; set; }
    }
}
