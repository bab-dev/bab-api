using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class BeneficiaryFamilyParameters : QueryStringParameters
    {
        public BeneficiaryFamilyParameters()
        {
            OrderBy = "RegistrationDate desc";
        }

        //Filtering params
        public int? HousingType { get; set; } = null;

        //Searching param
        public string Name { get; set; } = null;
    }
}
