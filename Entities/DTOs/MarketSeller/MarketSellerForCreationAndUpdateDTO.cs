using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class MarketSellerForCreationAndUpdateDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "MarketName is required")]
        public string MarketName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "IDProductCategory is required")]
        public Guid IDProductCategory { get; set; }
    }
}
