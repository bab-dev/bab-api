using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.BeneficiaryOrganization
{
    public class BeneficiaryOrganizationDTO
    {
        public Guid Id { get; set; }
        public int IDCoordinator { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationType { get; set; }
        public string Program { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public string LegalRepresentative { get; set; }
        public string Address { get; set; }
        public int PhoneNumber { get; set; }
        public int TotalPopulation { get; set; }
        public string Observations { get; set; }
    }
}
