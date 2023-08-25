using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("volunteer")]
    public class Volunteer
    {
        [Key]
        [Column("IDVolunteer")]
        public int Id{ get; set; }

        [ForeignKey(nameof(Person))]
        public Guid IDPerson { get; set; }

        [ForeignKey(nameof(Department))]
        public Guid IDDepartment { get; set; }

        public bool IsOlder { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public VolunteerRole Role { get; set; }

        [Required(ErrorMessage = "VolunteerCategory is required")]
        public VolunteerCategory Category { get; set; }

        public string Group { get; set; }

        [Required(ErrorMessage = "IsVaccinated is required")]
        public bool IsVaccinated { get; set; }

        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }

        public RoleObj GetRole()
        {
            RoleObj roleObj = new RoleObj
            {
                roleName = Role.ToString(),
                roleValue = (int)Role
            };
            return roleObj;
        }
    }
}
