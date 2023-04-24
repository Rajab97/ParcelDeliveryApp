using DeliveryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryManagementService.Domain.EntityConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(m => m.OrderNumber).HasMaxLength(50).IsRequired(true);
            builder.HasIndex(m => m.OrderNumber).IsUnique();
            builder.Property(m => m.CustomerId).IsRequired(true);
            builder.Property(m => m.OrderStatus).IsRequired(true);
            builder.Property(m => m.ShippingAddress).HasMaxLength(250).IsRequired(true);
            builder.Property(m => m.BillingAddress).HasMaxLength(250).IsRequired(true);
            builder.Property(m => m.CourierUserName).HasMaxLength(50).IsRequired(false);
            builder.Property(m => m.CourierFullName).HasMaxLength(100).IsRequired(false);
            builder.Property(m => m.CustomerFullName).HasMaxLength(100).IsRequired(false);
            builder.Property(m => m.CourierId).HasMaxLength(100).IsRequired(true);
            builder.Property(m => m.CustomerId).IsRequired(true);
            builder.Property(m => m.AssignedBy).HasMaxLength(100).IsRequired(false);
            builder.Property(m => m.OrderStatus).HasMaxLength(50).IsRequired(true);


            builder.HasMany(m => m.DeliveryHostories).WithOne(m => m.Order);
        }
    }
}
