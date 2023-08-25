using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IMarketSellerRepository : IRepositoryBase<MarketSeller>
    {
        PagedList<MarketSeller> GetAllMarketSellers([Optional] MarketSellerParameters parameters);
        MarketSeller GetMarketSellerById(Guid idMarketSeller);
        void CreateMarketSeller(MarketSeller marketSeller);
        void UpdateMarketSeller(MarketSeller marketSeller);
        void DeleteMarketSeller(MarketSeller marketSeller);
    }
}
