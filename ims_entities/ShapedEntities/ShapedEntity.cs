using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.ShapedEntities
{
    public abstract class ShapedEntity
    {
        public Entity Entity { get; set; }

        public ShapedEntity()
        {
            Entity = new Entity();
        }
    }
}
