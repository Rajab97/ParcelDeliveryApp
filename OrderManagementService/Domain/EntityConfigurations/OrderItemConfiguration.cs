using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Domain.EntityConfigurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(m => m.ProductNumber).HasMaxLength(50).IsRequired(true);
            builder.Property(m => m.Price).IsRequired(true);
            builder.Property(m => m.Quantity).IsRequired(true);
        }
    }
}
