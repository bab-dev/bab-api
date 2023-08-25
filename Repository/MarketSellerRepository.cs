using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MarketSellerRepository : RepositoryBase<MarketSeller>, IMarketSellerRepository
    {
        private readonly ISortHelper<MarketSeller> _sortHelper;
        public MarketSellerRepository(RepositoryContext repositoryContext, ISortHelper<MarketSeller> sortHelper)
            : base(repositoryContext)
        {
            _sortHelper = sortHelper;
        }

        private void SearchByName(ref IQueryable<MarketSeller> marketSellers, string name)
        {
            if (!marketSellers.Any() || string.IsNullOrWhiteSpace(name))
                return;
            
            marketSellers = marketSellers.Where(marketSeller => 
                marketSeller.Name.ToLower().Contains(name.Trim().ToLower()) ||
                marketSeller.LastName.ToLower().Contains(name.Trim().ToLower()) ||
                (marketSeller.Name + " " + marketSeller.LastName).ToLower().Contains(name.ToLower()));
        }

        public PagedList<MarketSeller> GetAllMarketSellers([Optional] MarketSellerParameters parameters)
        {
            var marketSellers = FindByCondition(marketSeller => (marketSeller.IDProductCategory == parameters.IDProductCategory || parameters.IDProductCategory == null));

            if (parameters is not null)
            {
                SearchByName(ref marketSellers, parameters.Name);
                var sortedMarketSellers = _sortHelper.ApplySort(marketSellers, parameters.OrderBy);
                if (parameters.PageSize.HasValue)
                {
                    return PagedList<MarketSeller>.ToPagedList(sortedMarketSellers, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<MarketSeller>.ToPagedList(sortedMarketSellers, 1, sortedMarketSellers.Count());
                }
            } else
            {
                var sortedMarketSellers = _sortHelper.ApplySort(marketSellers, "Name");
                return PagedList<MarketSeller>.ToPagedList(sortedMarketSellers, 1, 10);
            }
        }

        public MarketSeller GetMarketSellerById(Guid idMarketSeller)
        {
            return FindByCondition(marketSeller => marketSeller.Id.Equals(idMarketSeller))
                    .FirstOrDefault();
        }

        public void CreateMarketSeller(MarketSeller marketSeller)
        {
            marketSeller.Id = Guid.NewGuid();
            Create(marketSeller);
            Save();
        }

        public void UpdateMarketSeller(MarketSeller marketSeller)
        {
            Update(marketSeller);
            Save();
        }

        public void DeleteMarketSeller(MarketSeller marketSeller)
        {
            Delete(marketSeller);
            Save();
        }
    }
}
