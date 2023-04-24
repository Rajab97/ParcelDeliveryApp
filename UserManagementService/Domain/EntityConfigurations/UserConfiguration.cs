using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementService.Domain.Entities;

namespace UserManagementService.Domain.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
           builder.Property(x => x.FirstName).HasMaxLength(25).IsRequired(true);
           builder.Property(x => x.LastName).HasMaxLength(25).IsRequired(true);
        }
    }
}
