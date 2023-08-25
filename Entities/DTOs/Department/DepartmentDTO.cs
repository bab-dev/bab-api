using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class DepartmentDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        public string DepartmentName { get; set; }
    }
}
