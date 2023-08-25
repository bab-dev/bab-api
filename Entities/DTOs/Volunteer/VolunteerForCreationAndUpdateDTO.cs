using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VolunteerForCreationAndUpdateDTO
    {
        [ForeignKey(nameof(Person))]
        public Guid IDPerson { get; set; }

        [ForeignKey(nameof(Department))]
        public Guid IDDepartment { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public VolunteerRole Role { get; set; }

        [Required(ErrorMessage = "VolunteerCategory is required")]
        public VolunteerCategory Category { get; set; }

        public string Group { get; set; }

        [Required(ErrorMessage = "IsVaccinated is required")]
        public bool IsVaccinated { get; set; }
    }
}
