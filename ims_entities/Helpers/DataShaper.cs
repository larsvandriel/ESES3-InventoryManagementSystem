using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Entities.ShapedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Helpers
{
    public class DataShaper<T>: IDataShaper<T>
    {
        public PropertyInfo[] Properties { get; set; }

        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);

            return FetchData(entities, requiredProperties);
        }

        public ShapedEntity ShapeData(T entity, string fieldString)
        {
            var requiredProperties = GetRequiredProperties(fieldString);

            return FetchDataForEntity(entity, requiredProperties);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredProperties = new List<PropertyInfo>();

            if (!string.IsNullOrWhiteSpace(fieldsString))
            {
                var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var field in fields)
                {
                    var property = Properties.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                    if (property == null)
                    {
                        continue;
                    }

                    requiredProperties.Add(property);
                }
            }
            else
            {
                requiredProperties = Properties.ToList();
            }
            return requiredProperties;
        }

        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ShapedEntity>();

            foreach (var entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }

            return shapedData;
        }

        private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            ShapedEntity shapedObject;
            if (entity is InventoryItem)
            {
                var shapedInventoryItemEntity = new ShapedInventoryItemEntity();

                var objectProperty = entity.GetType().GetProperty("InventoryId");
                shapedInventoryItemEntity.InventoryId = (Guid)objectProperty.GetValue(entity);

                objectProperty = entity.GetType().GetProperty("ProductId");
                shapedInventoryItemEntity.ProductId = (Guid)objectProperty.GetValue(entity);

                shapedObject = shapedInventoryItemEntity;
            }
            else if (entity is InventoryItemNotificationThreshold)
            {
                var shapedInventoryItemNotificationThresholdEntity = new ShapedInventoryItemNotificationThresholdEntity();

                var objectProperty = entity.GetType().GetProperty("InventoryId");
                shapedInventoryItemNotificationThresholdEntity.InventoryId = (Guid)objectProperty.GetValue(entity);

                objectProperty = entity.GetType().GetProperty("ProductId");
                shapedInventoryItemNotificationThresholdEntity.ProductId = (Guid)objectProperty.GetValue(entity);

                objectProperty = entity.GetType().GetProperty("ThresholdAmount");
                shapedInventoryItemNotificationThresholdEntity.ThresholdAmount = (int)objectProperty.GetValue(entity);

                objectProperty = entity.GetType().GetProperty("ThresholdType");
                shapedInventoryItemNotificationThresholdEntity.ThresholdType = (ThresholdType)objectProperty.GetValue(entity);

                objectProperty = entity.GetType().GetProperty("NotificationMethod");
                shapedInventoryItemNotificationThresholdEntity.NotificationMethod = (string)objectProperty.GetValue(entity);

                shapedObject = shapedInventoryItemNotificationThresholdEntity;
            }
            else
            {
                var normalShapedObject = new NormalShapedEntity();

                var objectProperty = entity.GetType().GetProperty("Id");
                normalShapedObject.Id = (Guid)objectProperty.GetValue(entity);

                shapedObject = normalShapedObject;
            }

            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }

            return shapedObject;
        }
    }
}
