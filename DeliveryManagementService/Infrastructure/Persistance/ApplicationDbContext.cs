using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DeliveryManagementService.Domain.Entities;

namespace DeliveryManagementService.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryHostory> DeliveryHostories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var assambly = typeof(Program).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assambly);
        }
    }
}
