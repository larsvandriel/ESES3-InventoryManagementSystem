using InventoryManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Extensions
{
    public static class InventoryExtensions
    {
        public static void Map(this Inventory dbInventory, Inventory inventory)
        {
            dbInventory.Name = inventory.Name;
        }
    }
}
