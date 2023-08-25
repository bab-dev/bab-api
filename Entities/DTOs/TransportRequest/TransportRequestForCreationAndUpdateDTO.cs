using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class TransportRequestForCreationAndUpdateDTO
    {

        [ForeignKey(nameof(Department))]
        public Guid IDDepartment { get; set; }

        [ForeignKey(nameof(Volunteer))]
        public int IDCoordinator { get; set; }
        public TransportType TransportType { get; set; }
        public int PlaceType { get; set; }
        public Guid IDPlace { get; set; }
        public DateTime Date { get; set; }
        public string TimeRange { get; set; }
        public string Detail { get; set; }
        public string Observations { get; set; }
        public PriorityType Priority { get; set; }
        public StatusType Status { get; set; }
        public string CommentByDirector { get; set; }
    }
}
