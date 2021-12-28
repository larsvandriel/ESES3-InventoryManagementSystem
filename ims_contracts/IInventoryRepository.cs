using InventoryManagementSystem.Entities.Helpers;
using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Entities.Parameters;
using InventoryManagementSystem.Entities.ShapedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Contracts
{
    public interface IInventoryRepository: IRepositoryBase<Inventory>
    {
        PagedList<ShapedEntity> GetAllInventories(InventoryParameters inventoryParameters);
        ShapedEntity GetInventoryById(Guid inventoryId, string fields);
        Inventory GetInventoryById(Guid inventoryId);
        void CreateInventory(Inventory inventory);
        void UpdateInventory(Inventory dbInventory, Inventory inventory);
        void DeleteInventory(Inventory inventory);
    }
}
