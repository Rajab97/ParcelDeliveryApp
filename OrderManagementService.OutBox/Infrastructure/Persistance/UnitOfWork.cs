using SharedLibrary.Domain;

namespace OrderManagementService.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CompleteAsync()
        {
           await _dbContext.SaveChangesAsync();
        }
    }
}
