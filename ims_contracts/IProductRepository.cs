using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Entities.ShapedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Contracts
{
    public interface IProductRepository: IRepositoryBase<Product>
    {
        ShapedEntity GetProductById(Guid productId, string fields);
        Product GetProductById(Guid productId);
        void CreateProduct(Product product);
        void UpdateProduct(Product dbProduct, Product product);
        void DeleteProduct(Product product);
    }
}
