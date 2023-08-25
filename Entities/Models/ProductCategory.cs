using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("product_category")]
    public class ProductCategory
    {
        [Key]
        [Column("IDProductCategory")]
        public Guid Id { get; set; }
        public string ProductCategoryName { get; set; }
    }
}
