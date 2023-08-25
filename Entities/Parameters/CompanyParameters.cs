using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class CompanyParameters : QueryStringParameters
    {
        public CompanyParameters()
        {
            OrderBy = "BusinessName";
        }

        //Searching param
        public string CompanyComercialName { get; set; } = null;
    }
}
