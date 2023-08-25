using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.PersonBeneficiaryFamily
{
    public class PersonBeneficiaryFamilyDTO
    {
        public PersonDTO Person { get; set; }
        public BeneficiaryFamilyDTO BeneficiaryFamily { get; set; }
        public IEnumerable<BeneficiaryFamilyMemberDTO>? Members { get; set; }
    }
}
