using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Goals;
using ExpenseTracker.Dtos.Categories;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeGoalRepository : IGoalRepository
    {
        private readonly ISessionFactory _sf;

        public NativeGoalRepository(ISessionFactory sf)
        {
            _sf = sf;
        }

        public async Task CreateAsync(Goal goal)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                await s.SaveAsync(goal);
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<Goal?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.Id == id)
                .Fetch(g => g.Category)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Goal goal)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                await s.UpdateAsync(goal);
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                var goal = await s.GetAsync<Goal>(id);
                if (goal != null)
                {
                    await s.DeleteAsync(goal);
                }
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Goal>> GetAllAsync()
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>().ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetByUserIdAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.UserId == userId)
                .Fetch(g => g.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetByUserIdAndStatusAsync(Guid userId, ExpenseTracker.Dtos.Models.GoalStatus status)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.UserId == userId && g.Status == status)
                .Fetch(g => g.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetByCategoryIdAsync(Guid categoryId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetActiveGoalsByUserIdAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.UserId == userId)
                .Where(g => g.GoalStatusString == "Active")
                .Fetch(g => g.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetCompletedGoalsByUserIdAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.UserId == userId)
                .Where(g => g.GoalStatusString == "Completed")
                .Fetch(g => g.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoalProgressDto>> GetGoalProgressByUserIdAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            var goals = await s.Query<Goal>()
                .Where(g => g.UserId == userId)
                .Where(g => g.GoalStatusString == "Active")
                .Fetch(g => g.Category)
                .ToListAsync();

            return goals.Select(goal => new GoalProgressDto
            {
                Id = goal.Id,
                GoalId = goal.Id,
                Name = goal.Name,
                Description = goal.Description,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                RemainingAmount = goal.TargetAmount - goal.CurrentAmount,
                PercentageComplete = (int)((goal.CurrentAmount / goal.TargetAmount) * 100),
                CategoryId = goal.CategoryId,
                CategoryName = goal.Category?.Name ?? "Unknown",
                Category = goal.Category != null ? new ExpenseTracker.Dtos.Categories.CategoryDto(
                    goal.Category.Id,
                    goal.Category.UserId,
                    goal.Category.Name,
                    goal.Category.CategoryType,
                    goal.Category.Description,
                    goal.Category.CreatedAt,
                    goal.Category.UpdatedAt,
                    null
                ) : null,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                Tag = goal.Tag,
                Status = goal.Status,
                Priority = goal.Priority,
                StatusColor = GetStatusColor(goal.Status),
                PriorityColor = GetPriorityColor(goal.Priority),
                DaysRemaining = goal.EndDate.HasValue ? (int)(goal.EndDate.Value - DateTime.UtcNow).TotalDays : 0,
                IsOverdue = goal.EndDate.HasValue && goal.EndDate.Value < DateTime.UtcNow,
                CreatedAt = goal.CreatedAt,
                UpdatedAt = goal.UpdatedAt
            }).ToList();
        }

        private string GetStatusColor(ExpenseTracker.Dtos.Models.GoalStatus status)
        {
            return status switch
            {
                ExpenseTracker.Dtos.Models.GoalStatus.Active => "success",
                ExpenseTracker.Dtos.Models.GoalStatus.Paused => "warning",
                ExpenseTracker.Dtos.Models.GoalStatus.Completed => "info",
                ExpenseTracker.Dtos.Models.GoalStatus.Cancelled => "error",
                _ => "default"
            };
        }

        public async Task<bool> HasActiveGoalAsync(Guid userId, Guid categoryId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.UserId == userId)
                .Where(g => g.CategoryId == categoryId)
                .Where(g => g.GoalStatusString == "Active")
                .AnyAsync();
        }

        public async Task<bool> HasActiveGoalForCategoryExcludingAsync(Guid userId, Guid categoryId, Guid excludeGoalId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Goal>()
                .Where(g => g.UserId == userId)
                .Where(g => g.CategoryId == categoryId)
                .Where(g => g.Id != excludeGoalId)
                .Where(g => g.GoalStatusString == "Active")
                .AnyAsync();
        }

        private string GetPriorityColor(ExpenseTracker.Dtos.Models.GoalPriority priority)
        {
            return priority switch
            {
                ExpenseTracker.Dtos.Models.GoalPriority.Low => "success",
                ExpenseTracker.Dtos.Models.GoalPriority.Medium => "warning",
                ExpenseTracker.Dtos.Models.GoalPriority.High => "error",
                _ => "default"
            };
        }
    }
}
