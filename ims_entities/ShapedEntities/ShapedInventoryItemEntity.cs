using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.ShapedEntities
{
    public class ShapedInventoryItemEntity: ShapedEntity
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
    }
}
