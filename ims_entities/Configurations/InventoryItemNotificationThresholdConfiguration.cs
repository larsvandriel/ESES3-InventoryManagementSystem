using InventoryManagementSystem.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Entities.Configurations
{
    public class InventoryItemNotificationThresholdConfiguration : IEntityTypeConfiguration<InventoryItemNotificationThreshold>
    {
        public void Configure(EntityTypeBuilder<InventoryItemNotificationThreshold> builder)
        {
            builder.HasKey(iint => new { iint.InventoryId, iint.ProductId, iint.ThresholdAmount, iint.ThresholdType, iint.NotificationMethod });
        }
    }
}
