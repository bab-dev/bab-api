using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("beneficiary_organization")]
    public class BeneficiaryOrganization
    {
        [Key]
        [Column("IDBeneficiaryOrganization")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "IDCoordiator is required")]
        [ForeignKey(nameof(Volunteer))]
        public int IDCoordinator { get; set; }

        [Required(ErrorMessage = "OrganizationName is required")]
        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "OrganizationType is required")]
        public BeneficiaryType OrganizationType { get; set; }

        public string Program { get; set; }

        [Required(ErrorMessage = "ContractStartDate is required")]
        public DateTime ContractStartDate { get; set; }

        [Required(ErrorMessage = "ContractEndDate is required")]
        public DateTime ContractEndDate { get; set; }

        [Required(ErrorMessage = "LegalRepresentative is required")]
        public string LegalRepresentative { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "TotalPopulation is required")]
        public int TotalPopulation { get; set; }

        public string Observations { get; set; }
    }
}
