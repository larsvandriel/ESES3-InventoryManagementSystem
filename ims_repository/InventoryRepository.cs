using InventoryManagementSystem.Contracts;
using InventoryManagementSystem.Entities;
using InventoryManagementSystem.Entities.Extensions;
using InventoryManagementSystem.Entities.Helpers;
using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Entities.Parameters;
using InventoryManagementSystem.Entities.ShapedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository
{
    public class InventoryRepository: RepositoryBase<Inventory>, IInventoryRepository
    {
        private readonly ISortHelper<Inventory> _sortHelper;

        private readonly IDataShaper<Inventory> _dataShaper;

        public InventoryRepository(RepositoryContext repositoryContext, ISortHelper<Inventory> sortHelper, IDataShaper<Inventory> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public void CreateInventory(Inventory inventory)
        {
            Create(inventory);
        }

        public void DeleteInventory(Inventory inventory)
        {
            Delete(inventory);
        }

        public PagedList<ShapedEntity> GetAllInventories(InventoryParameters inventoryParameters)
        {
            var inventories = FindAll();

            SearchByName(ref inventories, inventoryParameters.Name);

            var sortedInventories = _sortHelper.ApplySort(inventories, inventoryParameters.OrderBy);
            var shapedInventories = _dataShaper.ShapeData(sortedInventories, inventoryParameters.Fields).AsQueryable();

            return PagedList<ShapedEntity>.ToPagedList(shapedInventories, inventoryParameters.PageNumber, inventoryParameters.PageSize);
        }

        public ShapedEntity GetInventoryById(Guid inventoryId, string fields)
        {
            var inventory = FindByCondition(inventory => inventory.Id.Equals(inventoryId)).FirstOrDefault();

            if (inventory == null)
            {
                inventory = new Inventory();
            }

            return _dataShaper.ShapeData(inventory, fields);
        }

        public Inventory GetInventoryById(Guid inventoryId)
        {
            return FindByCondition(i => i.Id.Equals(inventoryId)).FirstOrDefault();
        }

        public void UpdateInventory(Inventory dbInventory, Inventory inventory)
        {
            dbInventory.Map(inventory);
            Update(dbInventory);
        }

        private void SearchByName(ref IQueryable<Inventory> inventories, string inventoryName)
        {
            if (!inventories.Any() || string.IsNullOrWhiteSpace(inventoryName))
            {
                return;
            }

            inventories = inventories.Where(i => i.Name.ToLower().Contains(inventoryName.Trim().ToLower()));
        }
    }
}
