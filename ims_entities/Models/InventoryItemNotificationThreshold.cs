using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Models
{
    public class InventoryItemNotificationThreshold
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int ThresholdAmount { get; set; }
        public ThresholdType ThresholdType { get; set; }
        public string NotificationMethod { get; set; }
    }
}
