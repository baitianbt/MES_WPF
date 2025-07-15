 using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        Task<T?> GetByIdAsync(object id);
        
        Task<T> AddAsync(T entity);
        
        Task<T> UpdateAsync(T entity);
        
        Task DeleteAsync(T entity);
        
        Task DeleteByIdAsync(object id);
        
        Task<int> SaveChangesAsync();
    }
}