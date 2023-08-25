using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductCategoryRepository : IRepositoryBase<ProductCategory>
    {
        IEnumerable<ProductCategory> GetAllProductCategories();
        ProductCategory GetProductCategoryById(Guid idProductCategory);
        void CreateProductCategory(ProductCategory productCategory);
        void UpdateProductCategory(ProductCategory productCategory);
        void DeleteProductCategory(ProductCategory productCategory);
    }
}
