using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class BeneficiaryFamilyDTO
    {
        public Guid Id { get; set; }
        public Guid IDPerson { get; set; }
        public int BeneficiaryType { get; set; }
        public int HousingType { get; set; }
        public string HousingTypeName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Zone { get; set; }
        public string Observations { get; set; }
    }
}
