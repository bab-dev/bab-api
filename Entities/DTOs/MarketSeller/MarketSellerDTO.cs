using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class MarketSellerDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string MarketName { get; set; }
        public int PhoneNumber { get; set; }
        public Guid IDProductCategory { get; set; }
        public string ProductCategoryName { get; set; }
    }
}
