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
    public interface IInventoryItemNotificationThresholdRepository: IRepositoryBase<InventoryItemNotificationThreshold>
    {
        PagedList<ShapedEntity> GetAllInventoryItemNotificationThresholds(InventoryItemNotificationThresholdParameters inventoryItemNotificationThresholdParameters);
        ShapedEntity GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod(Guid inventoryId, Guid productId, int thresholdAmount, ThresholdType thresholdType, string notificationMethod, string fields);
        InventoryItemNotificationThreshold GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod(Guid inventoryId, Guid productId, int thresholdAmount, ThresholdType thresholdType, string notificationMethod);
        void CreateInventoryItemNotificationThreshold(InventoryItemNotificationThreshold inventoryItemNotificationThreshold);
        void DeleteInventoryItemNotificationThreshold(InventoryItemNotificationThreshold inventoryItemNotificationThreshold);
    }
}
