using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Goals;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public interface IGoalService
    {
        Task<GoalDto> CreateGoalAsync(Guid userId, CreateGoalDto createGoalDto);
        Task<GoalDto> UpdateGoalAsync(Guid userId, UpdateGoalDto updateGoalDto);
        Task DeleteGoalAsync(Guid userId, Guid goalId);
        Task<GoalDto?> GetGoalByIdAsync(Guid userId, Guid goalId);
        Task<IEnumerable<GoalDto>> GetGoalsByUserIdAsync(Guid userId);
        Task<IEnumerable<GoalDto>> GetActiveGoalsByUserIdAsync(Guid userId);
        Task<IEnumerable<GoalDto>> GetCompletedGoalsByUserIdAsync(Guid userId);
        Task<IEnumerable<GoalProgressDto>> GetGoalProgressByUserIdAsync(Guid userId);
        Task<bool> ExistsAsync(Guid userId, Guid goalId);
    }

    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _goalRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;

        public GoalService(IGoalRepository goalRepository, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
        {
            _goalRepository = goalRepository;
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<GoalDto> CreateGoalAsync(Guid userId, CreateGoalDto createGoalDto)
        {
            // Validate category exists and is of correct type
            var category = await _categoryRepository.GetByIdAsync(createGoalDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");
            
            if (category.CategoryType != CategoryType.TargetedSavingsGoal)
                throw new ArgumentException("Category must be of type TargetedSavingsGoal");

            // Check if user already has an active goal for this category
            if (createGoalDto.Status == GoalStatus.Active && await _goalRepository.HasActiveGoalAsync(userId, createGoalDto.CategoryId))
            {
                throw new InvalidOperationException("You already have an active goal for this category. Only one active goal per category is allowed.");
            }

            // Validate dates
            if (createGoalDto.EndDate.HasValue && createGoalDto.EndDate.Value <= createGoalDto.StartDate)
                throw new ArgumentException("End date must be after start date");

            // Validate amounts
            if (createGoalDto.TargetAmount <= 0)
                throw new ArgumentException("Target amount must be greater than 0");

            if (createGoalDto.CurrentAmount < 0)
                throw new ArgumentException("Current amount cannot be negative");

            if (createGoalDto.CurrentAmount > createGoalDto.TargetAmount)
                throw new ArgumentException("Current amount cannot exceed target amount");

            var goal = new Goal(
                Guid.NewGuid(),
                userId,
                createGoalDto.Name,
                createGoalDto.Description,
                createGoalDto.TargetAmount,
                createGoalDto.CurrentAmount,
                createGoalDto.CategoryId,
                createGoalDto.StartDate,
                createGoalDto.EndDate,
                createGoalDto.Tag,
                createGoalDto.Status,
                createGoalDto.Priority,
                DateTime.UtcNow,
                DateTime.UtcNow
            );

            await _goalRepository.CreateAsync(goal);
            return MapToDto(goal);
        }

        public async Task<GoalDto> UpdateGoalAsync(Guid userId, UpdateGoalDto updateGoalDto)
        {
            var existingGoal = await _goalRepository.GetByIdAsync(updateGoalDto.Id);
            if (existingGoal == null)
                throw new ArgumentException("Goal not found");

            if (existingGoal.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own goals");

            // Validate category exists and is of correct type
            var category = await _categoryRepository.GetByIdAsync(updateGoalDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");
            
            if (category.CategoryType != CategoryType.TargetedSavingsGoal)
                throw new ArgumentException("Category must be of type TargetedSavingsGoal");

            // Check if category is being changed and if new category already has an active goal
            if (updateGoalDto.CategoryId != existingGoal.CategoryId)
            {
                if (await _goalRepository.HasActiveGoalForCategoryExcludingAsync(userId, updateGoalDto.CategoryId, updateGoalDto.Id))
                {
                    throw new InvalidOperationException("You already have an active goal for this category. Only one active goal per category is allowed.");
                }
            }
            // If category is not changing, check if the goal status is being changed to Active and there's already an active goal
            else if (updateGoalDto.Status == GoalStatus.Active && existingGoal.Status != GoalStatus.Active)
            {
                // Category is same, but status is changing to Active - check if another active goal exists
                if (await _goalRepository.HasActiveGoalForCategoryExcludingAsync(userId, updateGoalDto.CategoryId, updateGoalDto.Id))
                {
                    throw new InvalidOperationException("You already have an active goal for this category. Only one active goal per category is allowed.");
                }
            }

            // Validate dates
            if (updateGoalDto.EndDate.HasValue && updateGoalDto.EndDate.Value <= updateGoalDto.StartDate)
                throw new ArgumentException("End date must be after start date");

            // Validate amounts
            if (updateGoalDto.TargetAmount <= 0)
                throw new ArgumentException("Target amount must be greater than 0");

            if (updateGoalDto.CurrentAmount < 0)
                throw new ArgumentException("Current amount cannot be negative");

            if (updateGoalDto.CurrentAmount > updateGoalDto.TargetAmount)
                throw new ArgumentException("Current amount cannot exceed target amount");

            // Update properties
            existingGoal.Name = updateGoalDto.Name;
            existingGoal.Description = updateGoalDto.Description;
            existingGoal.TargetAmount = updateGoalDto.TargetAmount;
            existingGoal.CurrentAmount = updateGoalDto.CurrentAmount;
            existingGoal.CategoryId = updateGoalDto.CategoryId;
            existingGoal.StartDate = updateGoalDto.StartDate;
            existingGoal.EndDate = updateGoalDto.EndDate;
            existingGoal.Tag = updateGoalDto.Tag;
            existingGoal.Status = updateGoalDto.Status;
            existingGoal.Priority = updateGoalDto.Priority;

            await _goalRepository.UpdateAsync(existingGoal);
            return MapToDto(existingGoal);
        }

        public async Task DeleteGoalAsync(Guid userId, Guid goalId)
        {
            var goal = await _goalRepository.GetByIdAsync(goalId);
            if (goal == null)
                throw new ArgumentException("Goal not found");

            if (goal.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own goals");

            await _goalRepository.DeleteAsync(goalId);
        }

        public async Task<GoalDto?> GetGoalByIdAsync(Guid userId, Guid goalId)
        {
            var goal = await _goalRepository.GetByIdAsync(goalId);
            if (goal == null || goal.UserId != userId)
                return null;

            return MapToDto(goal);
        }

        public async Task<IEnumerable<GoalDto>> GetGoalsByUserIdAsync(Guid userId)
        {
            var goals = await _goalRepository.GetByUserIdAsync(userId);
            return goals.Select(MapToDto);
        }

        public async Task<IEnumerable<GoalDto>> GetActiveGoalsByUserIdAsync(Guid userId)
        {
            var goals = await _goalRepository.GetActiveGoalsByUserIdAsync(userId);
            return goals.Select(MapToDto);
        }

        public async Task<IEnumerable<GoalDto>> GetCompletedGoalsByUserIdAsync(Guid userId)
        {
            var goals = await _goalRepository.GetCompletedGoalsByUserIdAsync(userId);
            return goals.Select(MapToDto);
        }

        public async Task<IEnumerable<GoalProgressDto>> GetGoalProgressByUserIdAsync(Guid userId)
        {
            var goals = await _goalRepository.GetActiveGoalsByUserIdAsync(userId);
            var progressDtos = new List<GoalProgressDto>();

            foreach (var goal in goals)
            {
                // Use the already-loaded category from the goal instead of fetching it again
                var progressDto = await CalculateGoalProgressAsync(goal, goal.Category);
                progressDtos.Add(progressDto);
            }

            return progressDtos.OrderByDescending(g => g.Priority).ThenByDescending(g => g.PercentageComplete);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid goalId)
        {
            var goal = await _goalRepository.GetByIdAsync(goalId);
            return goal != null && goal.UserId == userId;
        }

        private async Task<GoalProgressDto> CalculateGoalProgressAsync(Goal goal, Category? category)
        {
            // Calculate total from transactions for this goal's category since goal start date
            var transactionTotal = await _transactionRepository.GetTotalSpentByCategoryAndDateRangeAsync(
                goal.UserId,
                goal.CategoryId,
                goal.StartDate,
                DateTime.UtcNow
            );

            // Total current amount = opening balance (CurrentAmount) + transactions for this category
            var totalCurrentAmount = goal.CurrentAmount + transactionTotal;
            
            // Ensure it doesn't exceed target
            var actualCurrentAmount = totalCurrentAmount > goal.TargetAmount ? goal.TargetAmount : totalCurrentAmount;
            
            var remainingAmount = goal.TargetAmount - actualCurrentAmount;
            var percentageComplete = goal.TargetAmount > 0 ? (int)((actualCurrentAmount / goal.TargetAmount) * 100) : 0;
            
            // Cap at 100%
            if (percentageComplete > 100)
                percentageComplete = 100;
            
            var daysRemaining = 0;
            var isOverdue = false;
            
            if (goal.EndDate.HasValue)
            {
                var today = DateTime.Today;
                var endDate = goal.EndDate.Value.Date;
                daysRemaining = (endDate - today).Days;
                isOverdue = daysRemaining < 0;
            }

            return new GoalProgressDto
            {
                Id = goal.Id,
                GoalId = goal.Id,
                Name = goal.Name,
                Description = goal.Description,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = actualCurrentAmount,
                RemainingAmount = remainingAmount,
                PercentageComplete = percentageComplete,
                CategoryId = goal.CategoryId,
                CategoryName = category?.Name ?? "Unknown",
                Category = category != null ? new ExpenseTracker.Dtos.Categories.CategoryDto(
                    category.Id,
                    category.UserId,
                    category.Name,
                    category.CategoryType,
                    category.Description,
                    category.CreatedAt,
                    category.UpdatedAt,
                    null
                ) : null,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                Tag = goal.Tag,
                Status = goal.Status,
                Priority = goal.Priority,
                StatusColor = GetStatusColor(goal.Status),
                PriorityColor = GetPriorityColor(goal.Priority),
                DaysRemaining = daysRemaining,
                IsOverdue = isOverdue,
                CreatedAt = goal.CreatedAt,
                UpdatedAt = goal.UpdatedAt
            };
        }

        private string GetStatusColor(GoalStatus status)
        {
            return status switch
            {
                GoalStatus.Active => "success",
                GoalStatus.Paused => "warning",
                GoalStatus.Completed => "info",
                GoalStatus.Cancelled => "error",
                _ => "default"
            };
        }

        private string GetPriorityColor(GoalPriority priority)
        {
            return priority switch
            {
                GoalPriority.Low => "success",
                GoalPriority.Medium => "warning",
                GoalPriority.High => "error",
                _ => "default"
            };
        }

        private GoalDto MapToDto(Goal goal)
        {
            return new GoalDto
            {
                Id = goal.Id,
                UserId = goal.UserId,
                Name = goal.Name,
                Description = goal.Description,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                CategoryId = goal.CategoryId,
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
                CreatedAt = goal.CreatedAt,
                UpdatedAt = goal.UpdatedAt
            };
        }
    }
}
