using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EventDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string EventDescription { get; set; }
        public int EventTypeValue { get; set; }
        public string EventTypeName { get; set; }

        [ForeignKey(nameof(Department))]
        public Guid IDDepartment { get; set; }
        public string DepartmentName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Observations { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public int NumberOfVolunteersAssigned { get; set; }
    }
}
