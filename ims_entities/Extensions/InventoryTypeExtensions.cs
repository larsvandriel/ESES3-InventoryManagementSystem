using InventoryManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Extensions
{
    public static class InventoryTypeExtensions
    {
        public static void Map(this InventoryItem dbInventoryItem, InventoryItem inventoryItem)
        {
            dbInventoryItem.Amount = inventoryItem.Amount;
        }

        public static bool IsObjectNull(this InventoryItem inventoryItem)
        {
            return inventoryItem == null;
        }

        public static bool IsEmptyObject(this InventoryItem inventoryItem)
        {
            return inventoryItem.ProductId.Equals(Guid.Empty) || inventoryItem.Inventory.Equals(Guid.Empty);
        }
    }
}
