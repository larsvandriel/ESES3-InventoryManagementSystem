using InventoryManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.ShapedEntities
{
    public class ShapedInventoryItemNotificationThresholdEntity: ShapedEntity
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public int ThresholdAmount { get; set; }
        public ThresholdType ThresholdType { get; set; }
        public string NotificationMethod { get; set; }
    }
}
