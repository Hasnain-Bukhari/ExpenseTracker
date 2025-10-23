using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface ICategoryTypeRepository
    {
        Task<CategoryType?> GetByIdAsync(Guid id);
        Task<CategoryType?> GetByNameAsync(string name);
        Task<IList<CategoryType>> ListAsync();
        Task<IList<CategoryType>> ListActiveAsync();
        Task CreateAsync(CategoryType categoryType);
        Task UpdateAsync(CategoryType categoryType);
        Task DeleteAsync(Guid id);
    }
}
