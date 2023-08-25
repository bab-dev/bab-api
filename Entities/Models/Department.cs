using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("department")]
    public class Department
    {
        [Key]
        [Column("IDDepartment")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        public string DepartmentName { get; set; }
    }
}
