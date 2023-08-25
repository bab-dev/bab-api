using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class TripParameters : QueryStringParameters
    {
        public TripParameters()
        {
            OrderBy = "Date";
        }

        //Filtering params
        public int? IDCoordinator { get; set; } = null;
        public Guid? IDDepartment { get; set; } = null;
        public string? Vehicule { get; set; } = null;
        public int? Month { get; set; } = null;
        public int? Year { get; set; } = null;
        public int? TransportType { get; set; } = null;
        public string? DeparturePlace { get; set; } = null;
        public string? ArrivalPlace { get; set; } = null;

        //Searching param
        public string Place { get; set; } = null;
    }
}
