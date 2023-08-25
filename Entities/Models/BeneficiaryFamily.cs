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
    [Table("beneficiary_family")]
    public class BeneficiaryFamily
    {
        [Key]
        [Column("IDBeneficiaryFamily")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "IDPerson is required")]
        [ForeignKey(nameof(Person))]
        public Guid IDPerson { get; set; }

        [Required(ErrorMessage = "BeneficiaryType is required")]
        public BeneficiaryType BeneficiaryType { get; set; }

        [Required(ErrorMessage = "TypeOfHousing is required")]
        public HousingType HousingType { get; set; }

        [Required(ErrorMessage = "RegistrationDate is required")]
        public DateTime RegistrationDate { get; set; }

        [Required(ErrorMessage = "Zone is required")]
        public string Zone { get; set; }

        public string Observations { get; set; }
    }
}
