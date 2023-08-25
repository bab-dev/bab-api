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
    public class ProductCategoryRepository : RepositoryBase<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public IEnumerable<ProductCategory> GetAllProductCategories()
        {
            return FindAll()
                .OrderBy(productCategory => productCategory.Id)
                .ToList();
        }

        public ProductCategory GetProductCategoryById(Guid idProductCategory)
        {
            return FindByCondition(productCategory => productCategory.Id.Equals(idProductCategory))
                    .FirstOrDefault();
        }

        public void CreateProductCategory(ProductCategory productCategory)
        {
            productCategory.Id = Guid.NewGuid();
            Create(productCategory);
            Save();
        }

        public void UpdateProductCategory(ProductCategory productCategory)
        {
            Update(productCategory);
            Save();
        }

        public void DeleteProductCategory(ProductCategory productCategory)
        {
            Delete(productCategory);
            Save();
        }
    }
}
