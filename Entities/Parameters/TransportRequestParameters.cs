using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class TransportRequestParameters: QueryStringParameters
    {
        public TransportRequestParameters()
        {
            OrderBy = "CreatedAt";
        }

        //Filtering params
        public Guid? IDDepartment { get; set; } = null;
        public int? TransportType { get; set; } = null;
        public DateTime? MinRequestDate { get; set; } = null;
        public DateTime? MaxRequestDate { get; set; } = null;
        public int? Priority { get; set; } = null;
        public int? Status { get; set; } = null;

        //Searching param
        public string Name { get; set; } = null;
    }
}
