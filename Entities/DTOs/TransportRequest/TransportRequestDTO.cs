using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class TransportRequestDTO
    {
        public Guid Id { get; set; }
        public Guid IDDepartment { get; set; }
        public string DepartmentName { get; set; }
        public string IDCoordinator { get; set; }
        public string CoordinatorName { get; set; }
        public string TransportTypeName { get; set; }
        public int PlaceType { get; set; }
        public string PlaceTypeName { get; set; }
        public Guid IDPlace { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public string TimeRange { get; set; }
        public string Detail { get; set; }
        public string Observations { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public string CommentByDirector { get; set; }
    }
}
