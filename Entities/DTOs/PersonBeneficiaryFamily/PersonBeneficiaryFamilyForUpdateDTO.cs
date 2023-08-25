using Entities.DTOs.BeneficiaryFamilyMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.PersonBeneficiaryFamily
{
    public class PersonBeneficiaryFamilyForUpdateDTO
    {
        public PersonForCreationAndUpdateDTO Person { get; set; }
        public BeneficiaryFamilyForCreationAndUpdateDTO BeneficiaryFamily { get; set; }

        public IEnumerable<BeneficiaryFamilyMemberForUpdateDTO>? Members { get; set; }
    }
}
