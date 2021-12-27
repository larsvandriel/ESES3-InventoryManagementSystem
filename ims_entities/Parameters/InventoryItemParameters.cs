using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Parameters
{
    public class InventoryItemParameters: QueryStringParameters
    {
        public InventoryItemParameters()
        {
            OrderBy = "InventoryId";
        }
    }
}
