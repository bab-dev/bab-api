using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class BeneficiaryOrganizationParameters : QueryStringParameters
    {
        public BeneficiaryOrganizationParameters()
        {
            OrderBy = "OrganizationName";
        }

        //Filtering params
        public int? OrganizationType { get; set; } = null;
        public int? IDCoordinator { get; set; } = null;

        //Searching param
        public string OrganizationName { get; set; } = null;
    }
}
