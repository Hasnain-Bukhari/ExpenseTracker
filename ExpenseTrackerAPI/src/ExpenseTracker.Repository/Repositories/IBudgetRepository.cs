using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface IBudgetRepository
    {
        Task CreateAsync(Budget budget);
        Task<Budget?> GetByIdAsync(Guid id);
        Task<Budget?> GetActiveBudgetAsync(Guid userId, Guid categoryId);
        Task<List<Budget>> GetActiveBudgetsByUserAsync(Guid userId);
        Task<List<Budget>> GetBudgetsByUserAsync(Guid userId);
        Task<List<Budget>> GetBudgetsByCategoryAsync(Guid userId, Guid categoryId);
        Task UpdateAsync(Budget budget);
        Task DeleteAsync(Guid id);
        Task<bool> HasActiveBudgetAsync(Guid userId, Guid categoryId);
    }
}
