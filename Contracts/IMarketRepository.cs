using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IMarketRepository
    {
        IEnumerable<Market> GetAllMarkets();
        Market GetMarketById(Guid idMarket);
        void CreateMarket(Market market);
        void UpdateMarket(Market market);
        void DeleteMarket(Market market);
    }
}
