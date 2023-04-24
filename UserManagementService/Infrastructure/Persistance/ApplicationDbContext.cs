using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Domain.Entities;
using UserManagementService.Domain.EntityConfigurations;

namespace UserManagementService.Infrastructure.Persistance
{
    public class ApplicationDbContext : IdentityDbContext<User,IdentityRole<int>,int>
    {
        public DbSet<User> Users { get; set; }
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var assambly = typeof(UserConfiguration).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assambly);
        }
    }
}
