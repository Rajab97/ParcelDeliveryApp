using SharedLibrary.Domain.Entities;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetIQueryable();
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IEnumerable<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        Task<T> GetByIdAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
