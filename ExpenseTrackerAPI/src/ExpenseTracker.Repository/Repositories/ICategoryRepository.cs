using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface ICategoryRepository
    {
        Task CreateAsync(Category category);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> GetByNameAndUserAsync(Guid userId, string name, string type);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
        Task<IList<Category>> ListByUserAsync(Guid userId);

        // Subcategories
        Task CreateSubAsync(SubCategory subCategory);
        Task<SubCategory?> GetSubByIdAsync(Guid id);
        Task<SubCategory?> GetSubByNameAsync(Guid categoryId, string name);
        Task UpdateSubAsync(SubCategory subCategory);
        Task DeleteSubAsync(Guid id);
        Task<IList<SubCategory>> ListSubsByCategoryAsync(Guid categoryId);
    }
}
