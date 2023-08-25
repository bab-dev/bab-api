using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VolunteerPersonDTO
    {
        public int IdVolunteer{ get; set; }

        [ForeignKey(nameof(Person))]
        public Guid IDPerson { get; set; }
        public string FirstName { get; set; }
        public string FirstSurname { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Occupation { get; set; }
        public string RoleName { get; set; }
        public int RoleValue { get; set; }
        public Guid IDDepartment { get; set; }
        public int CategoryValue { get; set; }
        public string Group { get; set; }
        public bool IsVaccinated { get; set; }
        public bool IsActive { get; set; }
    }
}
