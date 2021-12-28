using InventoryManagementSystem.Contracts;
using InventoryManagementSystem.Entities;
using InventoryManagementSystem.Entities.Helpers;
using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Entities.Parameters;
using InventoryManagementSystem.Entities.ShapedEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository
{
    public class InventoryItemNotificationThresholdRepository: RepositoryBase<InventoryItemNotificationThreshold>, IInventoryItemNotificationThresholdRepository
    {
        private readonly ISortHelper<InventoryItemNotificationThreshold> _sortHelper;

        private readonly IDataShaper<InventoryItemNotificationThreshold> _dataShaper;

        public InventoryItemNotificationThresholdRepository(RepositoryContext repositoryContext, ISortHelper<InventoryItemNotificationThreshold> sortHelper, IDataShaper<InventoryItemNotificationThreshold> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public void CreateInventoryItemNotificationThreshold(InventoryItemNotificationThreshold inventoryItemNotificationThreshold)
        {
            Create(inventoryItemNotificationThreshold);
        }

        public void DeleteInventoryItemNotificationThreshold(InventoryItemNotificationThreshold inventoryItemNotificationThreshold)
        {
            Delete(inventoryItemNotificationThreshold);
        }

        public PagedList<ShapedEntity> GetAllInventoryItemNotificationThresholds(InventoryItemNotificationThresholdParameters inventoryItemNotificationThresholdParameters)
        {
            var inventoryItemNotificationThreshold = FindAll().Include(inventoryItem => inventoryItem.Inventory).Include(inventoryItem => inventoryItem.Product);

            var sortedInventoryItemNotificationThreshold = _sortHelper.ApplySort(inventoryItemNotificationThreshold, inventoryItemNotificationThresholdParameters.OrderBy);
            var shapedInventoryItemNotificationThreshold = _dataShaper.ShapeData(sortedInventoryItemNotificationThreshold, inventoryItemNotificationThresholdParameters.Fields).AsQueryable();

            return PagedList<ShapedEntity>.ToPagedList(shapedInventoryItemNotificationThreshold, inventoryItemNotificationThresholdParameters.PageNumber, inventoryItemNotificationThresholdParameters.PageSize);
        }

        public ShapedEntity GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod(Guid inventoryId, Guid productId, int thresholdAmount, ThresholdType thresholdType, string notificationMethod, string fields)
        {
            var inventoryItemNotificationThreshold = FindByCondition(i => i.InventoryId == inventoryId && i.ProductId == productId && i.ThresholdAmount == thresholdAmount && i.ThresholdType.Equals(thresholdType) && i.NotificationMethod == notificationMethod).Include(inventoryItem => inventoryItem.Inventory).Include(inventoryItem => inventoryItem.Product).FirstOrDefault();

            if (inventoryItemNotificationThreshold == null)
            {
                inventoryItemNotificationThreshold = new InventoryItemNotificationThreshold();
            }

            return _dataShaper.ShapeData(inventoryItemNotificationThreshold, fields);
        }

        public InventoryItemNotificationThreshold GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod(Guid inventoryId, Guid productId, int thresholdAmount, ThresholdType thresholdType, string notificationMethod)
        {
            return FindByCondition(i => i.InventoryId == inventoryId && i.ProductId == productId && i.ThresholdAmount == thresholdAmount && i.ThresholdType.Equals(thresholdType) && i.NotificationMethod == notificationMethod).Include(inventoryItem => inventoryItem.Inventory).Include(inventoryItem => inventoryItem.Product).FirstOrDefault();
        }
    }
}
