using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Repositories
{
    public interface IRepository<T, TKey>
    {
        Task<T?> GetAsync(TKey id);
        Task<IEnumerable<T>> ListAsync();
        Task SaveAsync(T entity);
        Task DeleteAsync(TKey id);
    }
}
