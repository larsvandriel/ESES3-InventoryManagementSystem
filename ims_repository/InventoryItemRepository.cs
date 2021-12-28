using InventoryManagementSystem.Contracts;
using InventoryManagementSystem.Entities;
using InventoryManagementSystem.Entities.Extensions;
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
    public class InventoryItemRepository: RepositoryBase<InventoryItem>, IInventoryItemRepository
    {
        private readonly ISortHelper<InventoryItem> _sortHelper;

        private readonly IDataShaper<InventoryItem> _dataShaper;

        public InventoryItemRepository(RepositoryContext repositoryContext, ISortHelper<InventoryItem> sortHelper, IDataShaper<InventoryItem> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public void CreateInventoryItem(InventoryItem inventoryItem)
        {
            Create(inventoryItem);
        }

        public void DeleteInventoryItem(InventoryItem inventoryItem)
        {
            Delete(inventoryItem);
        }

        public PagedList<ShapedEntity> GetAllInventoryItems(InventoryItemParameters inventoryItemParameters)
        {
            var inventoryItems = FindAll().Include(inventoryItem => inventoryItem.Inventory).Include(inventoryItem => inventoryItem.Product);

            var sortedInventoryItems = _sortHelper.ApplySort(inventoryItems, inventoryItemParameters.OrderBy);
            var shapedInventoryItems = _dataShaper.ShapeData(sortedInventoryItems, inventoryItemParameters.Fields).AsQueryable();

            return PagedList<ShapedEntity>.ToPagedList(shapedInventoryItems, inventoryItemParameters.PageNumber, inventoryItemParameters.PageSize);
        }

        public ShapedEntity GetInventoryItemByInventoryIdAndProductId(Guid inventoryId, Guid productId, string fields)
        {
            var inventoryItem = FindByCondition(inventoryItem => inventoryItem.InventoryId == inventoryId && inventoryItem.ProductId == productId).Include(inventoryItem => inventoryItem.Inventory).Include(inventoryItem => inventoryItem.Product).FirstOrDefault();

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem();
            }

            return _dataShaper.ShapeData(inventoryItem, fields);
        }

        public InventoryItem GetInventoryItemByInventoryIdAndProductId(Guid inventoryId, Guid productId)
        {
            return FindByCondition(inventoryItem => inventoryItem.InventoryId == inventoryId && inventoryItem.ProductId == productId).Include(inventoryItem => inventoryItem.Inventory).Include(inventoryItem => inventoryItem.Product).FirstOrDefault();
        }

        public void UpdateInventoryItem(InventoryItem dbInventoryItem, InventoryItem inventoryItem)
        {
            dbInventoryItem.Map(inventoryItem);
            Update(dbInventoryItem);
        }
    }
}
