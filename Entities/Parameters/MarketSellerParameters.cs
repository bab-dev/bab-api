using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public class MarketSellerParameters : QueryStringParameters
    {
        public MarketSellerParameters()
        {
            OrderBy = "Name";
        }

        //Filtering params
        public Guid? IDProductCategory { get; set; } = null;

        //Searching param
        public string Name { get; set; } = null;
    }
}
