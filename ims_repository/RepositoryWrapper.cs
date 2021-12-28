using InventoryManagementSystem.Contracts;
using InventoryManagementSystem.Entities;
using InventoryManagementSystem.Entities.Helpers;
using InventoryManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository
{
    public class RepositoryWrapper: IRepositoryWrapper
    {
        private RepositoryContext _repoContext;

        private IInventoryRepository _inventory;
        private ISortHelper<Inventory> _inventorySortHelper;
        private IDataShaper<Inventory> _inventoryDataShaper;

        private IInventoryItemRepository _inventoryItem;
        private ISortHelper<InventoryItem> _inventoryItemSortHelper;
        private IDataShaper<InventoryItem> _inventoryItemDataShaper;

        private IInventoryItemNotificationThresholdRepository _inventoryItemNotificationThreshold;
        private ISortHelper<InventoryItemNotificationThreshold> _inventoryItemNotificationThresholdSortHelper;
        private IDataShaper<InventoryItemNotificationThreshold> _inventoryItemNotificationThresholdDataShaper;

        private IProductRepository _product;
        private ISortHelper<Product> _productSortHelper;
        private IDataShaper<Product> _productDataShaper;

        public IInventoryRepository Inventory
        {
            get
            {
                if (_inventory == null)
                {
                    _inventory = new InventoryRepository(_repoContext, _inventorySortHelper, _inventoryDataShaper);
                }

                return _inventory;
            }
        }

        public IInventoryItemRepository InventoryItem
        {
            get
            {
                if (_inventoryItem == null)
                {
                    _inventoryItem = new InventoryItemRepository(_repoContext, _inventoryItemSortHelper, _inventoryItemDataShaper);
                }

                return _inventoryItem;
            }
        }

        public IInventoryItemNotificationThresholdRepository InventoryItemNotificationThreshold
        {
            get
            {
                if (_inventoryItemNotificationThreshold == null)
                {
                    _inventoryItemNotificationThreshold = new InventoryItemNotificationThresholdRepository(_repoContext, _inventoryItemNotificationThresholdSortHelper, _inventoryItemNotificationThresholdDataShaper);
                }

                return _inventoryItemNotificationThreshold;
            }
        }

        public IProductRepository Product
        {
            get
            {
                if (_product == null)
                {
                    _product = new ProductRepository(_repoContext, _productSortHelper, _productDataShaper);
                }

                return _product;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext, ISortHelper<Inventory> inventorySortHelper, IDataShaper<Inventory> inventoryDataShaper, ISortHelper<InventoryItem> inventoryItemSortHelper, IDataShaper<InventoryItem> inventoryItemDataShaper, ISortHelper<InventoryItemNotificationThreshold> inventoryItemNotificationThresholdSortHelper, IDataShaper<InventoryItemNotificationThreshold> inventoryItemNotificationThresholdDataShaper, ISortHelper<Product> productSortHelper, IDataShaper<Product> productDataShaper)
        {
            _repoContext = repositoryContext;
            _inventorySortHelper = inventorySortHelper;
            _inventoryDataShaper = inventoryDataShaper;
            _inventoryItemSortHelper = inventoryItemSortHelper;
            _inventoryItemDataShaper = inventoryItemDataShaper;
            _inventoryItemNotificationThresholdSortHelper = inventoryItemNotificationThresholdSortHelper;
            _inventoryItemNotificationThresholdDataShaper = inventoryItemNotificationThresholdDataShaper;
            _productSortHelper = productSortHelper;
            _productDataShaper = productDataShaper;
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
