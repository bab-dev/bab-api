using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.NewPersonBeneficiaryFamily
{
    public class NewBeneficiaryMemberForCreationDTO
    {
        public Guid Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public char Gender { get; set; }
        public bool HasDisability { get; set; }
    }
}
