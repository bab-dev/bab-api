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
    [Table("beneficiary_family_member")]
    public class BeneficiaryFamilyMember
    {
        [Key]
        [Column("IDBeneficiaryFamilyMember")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "IDBeneficiaryFamily is required")]
        [ForeignKey(nameof(BeneficiaryFamily))]
        public Guid IDBeneficiary { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public char Gender { get; set; }

        [Required(ErrorMessage = "HasDisability is required")]
        public bool HasDisability { get; set; }
    }
}
