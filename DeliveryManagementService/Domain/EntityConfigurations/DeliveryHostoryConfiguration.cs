using DeliveryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryManagementService.Domain.EntityConfigurations
{
    public class DeliveryHostoryConfiguration : IEntityTypeConfiguration<DeliveryHostory>
    {
        public void Configure(EntityTypeBuilder<DeliveryHostory> builder)
        {
            builder.Property(m => m.Status).HasMaxLength(50).IsRequired(true);
            builder.Property(m => m.EventTime).IsRequired(true);
            builder.Property(m => m.OrderId).IsRequired(true);
        }
    }
}
