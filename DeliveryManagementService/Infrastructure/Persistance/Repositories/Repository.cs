using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedLibrary.Domain.Entities;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models;
using SharedLibrary.Models.Constants;
using System.Linq.Expressions;

namespace DeliveryManagementService.Infrastructure.Persistance.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        public readonly DbContext dbContext;

        public Repository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            try
            {
                return await dbContext.Set<T>().AsNoTracking().ToListAsync();
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            try
            {
                return await dbContext.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occured while getting data from database");
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                if (entity is IAuditEntity auditEntity)
                    auditEntity.CreatedAt = DateTime.Now;

                await dbContext.Set<T>().AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occured while adding {typeof(T).Name} to database");
                throw new ApplicationException($"Error occured while adding {typeof(T).Name} to database", ex);
            }
        }
        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                if (entity is IAuditEntity auditEntity)
                    auditEntity.UpdatedAt = DateTime.Now;

                dbContext.Set<T>().Update(entity);
                return await Task.FromResult(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error occured while update data");
                throw new ApplicationException($"Error occured while updating {typeof(T).Name} to database", ex);
            }
        }
        public async Task<T> DeleteAsync(T entity)
        {
            try
            {
                
                var dbEntity = await dbContext.Set<T>().FirstOrDefaultAsync(m=> m.Id == entity.Id);
                if (dbEntity == null)
                    throw new ApplicationException(ExceptionMessages.ExceptionOccured);

                if (dbEntity is ISoftDeletableEntity e)
                {
                    e.IsActive = false;
                    if (dbEntity is IAuditEntity ae)
                        ae.UpdatedAt = DateTime.Now;

                    dbContext.Set<T>().Update(dbEntity);
                }
                else
                    dbContext.Set<T>().Remove(dbEntity);

                return entity;
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex,"Error occured while delete data");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while delete data");
                throw new ApplicationException($"Error occured while deleting {typeof(T).Name} from database", ex);
            }
        }

        public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = dbContext.Set<T>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync();
                }
                else
                {
                    return await query.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,ExceptionMessages.ExceptionOccured);
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} from database", ex);
            }
        }

        public IQueryable<T> GetIQueryable()
        {
            try
            {
                return dbContext.Set<T>();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Exception occured while get data from database");
                throw new ApplicationException($"Error occured while getting {typeof(T).Name} to database", ex);
            }
        }

      
    }
}
