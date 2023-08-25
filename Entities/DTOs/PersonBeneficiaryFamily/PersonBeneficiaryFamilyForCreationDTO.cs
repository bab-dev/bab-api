using Entities.DTOs.NewPersonBeneficiaryFamily;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.PersonBeneficiaryFamily
{
    public class PersonBeneficiaryFamilyForCreationDTO
    {
        public PersonForCreationAndUpdateDTO Person { get; set; }
        public NewBeneficiaryFamilyForCreationDTO BeneficiaryFamily { get; set; }
        public IEnumerable<NewBeneficiaryMemberForCreationDTO>? Members { get; set; }
    }
}
