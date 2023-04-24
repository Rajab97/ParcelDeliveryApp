using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Domain.EntityConfigurations
{
    public class OutBoxMessageConfiguration : IEntityTypeConfiguration<OutBoxMessage>
    {
        public void Configure(EntityTypeBuilder<OutBoxMessage> builder)
        {
            builder.Property(m => m.Topic)
                .HasMaxLength(50).IsRequired(true);
            builder.Property(m=>m.Message).IsRequired(true);
            builder.Property(m=>m.Status).HasMaxLength(25).IsRequired(true);
        }
    }
}
