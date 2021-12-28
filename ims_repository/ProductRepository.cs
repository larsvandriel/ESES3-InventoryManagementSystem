using InventoryManagementSystem.Contracts;
using InventoryManagementSystem.Entities;
using InventoryManagementSystem.Entities.Extensions;
using InventoryManagementSystem.Entities.Helpers;
using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Entities.ShapedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository
{
    public class ProductRepository: RepositoryBase<Product>, IProductRepository
    {
        private readonly ISortHelper<Product> _sortHelper;

        private readonly IDataShaper<Product> _dataShaper;

        public ProductRepository(RepositoryContext repositoryContext, ISortHelper<Product> sortHelper, IDataShaper<Product> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public void CreateProduct(Product product)
        {
            Create(product);
        }

        public void DeleteProduct(Product product)
        {
            Delete(product);
        }

        public ShapedEntity GetProductById(Guid productId, string fields)
        {
            var product = FindByCondition(product => product.Id == productId).FirstOrDefault();

            if (product == null)
            {
                product = new Product();
            }

            return _dataShaper.ShapeData(product, fields);
        }

        public Product GetProductById(Guid productId)
        {
            return FindByCondition(product => product.Id == productId).FirstOrDefault();
        }

        public void UpdateProduct(Product dbProduct, Product product)
        {
            dbProduct.Map(product);
            Update(dbProduct);
        }
    }
}
