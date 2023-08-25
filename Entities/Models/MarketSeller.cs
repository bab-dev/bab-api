using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("market_seller")]
    public class MarketSeller
    {
        [Key]
        [Column("IDMarketSeller")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "MarketName is required")]
        public string MarketName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public int PhoneNumber { get; set; }


        [ForeignKey(nameof(ProductCategory))]
        [Required(ErrorMessage = "IdProductCategory is required")]
        public Guid IDProductCategory { get; set; }
    }
}
