using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class BeneficiaryFamilyForCreationAndUpdateDTO
    {
        public Guid IDPerson { get; set; }
        public int BeneficiaryType { get; set; }
        public int HousingType { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Zone { get; set; }
        public string Observations { get; set; }
    }
}
