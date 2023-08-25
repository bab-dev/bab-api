using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class EventParameters : QueryStringParameters
    {
        public EventParameters()
        {
            OrderBy = "Start desc";
        }

        //Filtering params
        public Guid? IDDepartment { get; set; } = null;

        //Searching param
        public string Title { get; set; } = null;
    }
}
