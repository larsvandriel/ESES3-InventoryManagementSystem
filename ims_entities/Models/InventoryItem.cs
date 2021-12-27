using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Models
{
    public class InventoryItem
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
    }
}
