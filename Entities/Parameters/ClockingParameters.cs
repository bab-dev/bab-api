using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class ClockingParameters : QueryStringParameters
    {
        public ClockingParameters()
        {
            OrderBy = "Date desc";
        }

        //Filtering params
        public int? IDVolunteer{ get; set; } = null;
        public int? Month { get; set; } = null;
        public int? Year { get; set; } = null;
    }
}
