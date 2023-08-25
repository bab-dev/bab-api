using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class VolunteerParameters : QueryStringParameters
    {
        public VolunteerParameters()
        {
            OrderBy = "Id";
        }

        //Filtering params
        public Guid? IDDepartment { get; set; } = null;
        public int? Role { get; set; } = null;
        public bool IsActive { get; set; } = true;

        //Searching param
        public string Name { get; set; }
    }
}
