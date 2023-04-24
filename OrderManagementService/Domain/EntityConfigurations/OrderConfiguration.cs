using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Domain.EntityConfigurations
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
            builder.Property(m => m.CustomerFirstName).HasMaxLength(50).IsRequired(false);
            builder.Property(m => m.CustomerLastName).HasMaxLength(50).IsRequired(false);
            builder.Property(m => m.CustomerId).IsRequired(true);
            builder.Property(m => m.AssignedBy).HasMaxLength(100).IsRequired(false);
            builder.Property(m => m.OrderStatus).HasMaxLength(50).IsRequired(true);


            builder.HasMany(m => m.OrderItems).WithOne(m => m.Order);
        }
    }
}
