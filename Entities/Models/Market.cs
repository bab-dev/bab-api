using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("market")]
    public class Market
    {
        [Key]
        [Column("IDMarket")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Market name is required")]
        public string MarketName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
    }
}
