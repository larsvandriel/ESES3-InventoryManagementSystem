using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Contracts
{
    public interface IRepositoryWrapper
    {
        IInventoryItemNotificationThresholdRepository InventoryItemNotificationThreshold { get; }
        IInventoryItemRepository InventoryItem { get; }
        IInventoryRepository Inventory { get; }
        IProductRepository Product { get; }
        void Save();
    }
}
