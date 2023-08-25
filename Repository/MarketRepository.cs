using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MarketRepository : RepositoryBase<Market>, IMarketRepository
    {
        public MarketRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public IEnumerable<Market> GetAllMarkets()
        {
            return FindAll()
                .OrderBy(market => market.MarketName)
                .ToList();
        }

        public Market GetMarketById(Guid idMarket)
        {
            return FindByCondition(market => market.Id.Equals(idMarket))
                    .FirstOrDefault();
        }

        public void CreateMarket(Market market)
        {
            market.Id = Guid.NewGuid();
            Create(market);
            Save();
        }

        public void UpdateMarket(Market market)
        {
            Update(market);
            Save();
        }

        public void DeleteMarket(Market market)
        {
            Delete(market);
            Save();
        }
    }
}
