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
    public interface IInventoryItemRepository: IRepositoryBase<InventoryItem>
    {
        PagedList<ShapedEntity> GetAllInventoryItems(InventoryItemParameters inventoryItemParameters);
        ShapedEntity GetInventoryItemByInventoryIdAndProductId(Guid inventoryId, Guid productId, string fields);
        InventoryItem GetInventoryItemByInventoryIdAndProductId(Guid inventoryId, Guid productId);
        void CreateInventoryItem(InventoryItem inventoryItem);
        void UpdateInventoryItem(InventoryItem dbInventoryItem, InventoryItem inventoryItem);
        void DeleteInventoryItem(InventoryItem inventoryItem);
    }
}
