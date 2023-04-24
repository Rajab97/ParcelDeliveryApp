using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<OutBoxMessage> OutBoxMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var assambly = typeof(Program).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assambly);
        }
    }
}
