using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Goals;

namespace ExpenseTracker.Repository.Repositories
{
    public interface IGoalRepository
    {
        Task<Goal?> GetByIdAsync(Guid id);
        Task<IEnumerable<Goal>> GetAllAsync();
        Task<IEnumerable<Goal>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Goal>> GetByUserIdAndStatusAsync(Guid userId, ExpenseTracker.Dtos.Models.GoalStatus status);
        Task<IEnumerable<Goal>> GetByCategoryIdAsync(Guid categoryId);
        Task CreateAsync(Goal goal);
        Task UpdateAsync(Goal goal);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Goal>> GetActiveGoalsByUserIdAsync(Guid userId);
        Task<IEnumerable<Goal>> GetCompletedGoalsByUserIdAsync(Guid userId);
        Task<IEnumerable<GoalProgressDto>> GetGoalProgressByUserIdAsync(Guid userId);
    }
}
