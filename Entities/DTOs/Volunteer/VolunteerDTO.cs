using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class VolunteerDTO
    {
        public int Id { get; set; }
        public Guid IDPerson { get; set; }
        public Guid IDDepartment { get; set; }
        public bool IsOlder { get; set; }
        public int Role { get; set; }
        public string RoleName { get; set; }
        public int Category { get; set; }
        public string CategoryName { get; set; }
        public string Group { get; set; }
        public bool IsVaccinated { get; set; }
        public bool IsActive { get; set; }
    }
}
