using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CompanyForCreationAndUpdateDTO
    {
        [Required(ErrorMessage = "CompanyComercialName is required")]
        public string CompanyComercialName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "BusinessName is required")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Representative is required")]
        public string Representative { get; set; }

        [Required(ErrorMessage = "RepresentativePosition is required")]
        public string RepresentativePosition { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string ImageURL { get; set; }
    }
}
