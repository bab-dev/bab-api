using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class VolunteerWorkStatisticsParameters : QueryStringParameters
    {
        public VolunteerWorkStatisticsParameters()
        {
            OrderBy = "Id";
        }

        //Filtering params
        public int? IDVolunteer { get; set; } = null;
    }
}
