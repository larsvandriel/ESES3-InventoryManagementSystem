using InventoryManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Extensions
{
    public static class InventoryItemNotificationThresholdExtensions
    {
        public static void Map(this InventoryItemNotificationThreshold dbInventoryItemNotificationThreshold, InventoryItemNotificationThreshold inventoryItemNotificationThreshold)
        {
            dbInventoryItemNotificationThreshold.ThresholdAmount = inventoryItemNotificationThreshold.ThresholdAmount;
            dbInventoryItemNotificationThreshold.ThresholdType = inventoryItemNotificationThreshold.ThresholdType;
            dbInventoryItemNotificationThreshold.NotificationMethod = inventoryItemNotificationThreshold.NotificationMethod;
        }

        public static bool IsObjectNull(this InventoryItemNotificationThreshold inventoryItemNotificationThreshold)
        {
            return inventoryItemNotificationThreshold == null;
        }

        public static bool IsEmptyObject(this InventoryItemNotificationThreshold inventoryItemNotificationThreshold)
        {
            return inventoryItemNotificationThreshold.ProductId.Equals(Guid.Empty) || inventoryItemNotificationThreshold.Inventory.Equals(Guid.Empty) || inventoryItemNotificationThreshold.ThresholdAmount < 0 || string.IsNullOrWhiteSpace(inventoryItemNotificationThreshold.NotificationMethod);
        }
    }
}
