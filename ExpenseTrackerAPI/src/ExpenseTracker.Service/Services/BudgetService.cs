using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Budgets;
using ExpenseTracker.Dtos.Categories;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public class BudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;

        public BudgetService(
            IBudgetRepository budgetRepository,
            ICategoryRepository categoryRepository,
            ITransactionRepository transactionRepository)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<BudgetDto> CreateBudgetAsync(Guid userId, CreateBudgetDto dto)
        {
            // Validate category exists and is expense type
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Category not found");
            }

            if (category.CategoryType != CategoryType.Expense)
            {
                throw new InvalidOperationException("Budget can only be created for expense categories");
            }

            // Check if user already has an active budget for this category
            if (await _budgetRepository.HasActiveBudgetAsync(userId, dto.CategoryId))
            {
                throw new InvalidOperationException("Active budget already exists for this category");
            }

            // Validate amount
            if (dto.Amount <= 0)
            {
                throw new ArgumentException("Budget amount must be greater than zero");
            }

            // Set effective from to start of current month
            var effectiveFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var budget = new Budget(
                Guid.NewGuid(),
                userId,
                dto.CategoryId,
                dto.Amount,
                effectiveFrom,
                null, // NULL = active
                true, // IsActive
                DateTime.Now,
                DateTime.Now
            );

            await _budgetRepository.CreateAsync(budget);

            return MapToDto(budget, category);
        }

        public async Task<BudgetDto> UpdateBudgetAsync(Guid userId, UpdateBudgetDto dto)
        {
            var currentBudget = await _budgetRepository.GetByIdAsync(dto.Id);
            if (currentBudget == null)
            {
                throw new ArgumentException("Budget not found");
            }

            if (currentBudget.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only update your own budgets");
            }

            if (!currentBudget.IsActive)
            {
                throw new InvalidOperationException("Cannot update completed budgets");
            }

            // Validate amount
            if (dto.Amount <= 0)
            {
                throw new ArgumentException("Budget amount must be greater than zero");
            }

            // Check if we're in the same month as effective_from
            var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            if (currentBudget.EffectiveFrom == currentMonth)
            {
                // Same month: Just update the amount
                currentBudget.Amount = dto.Amount;
                currentBudget.UpdatedAt = DateTime.Now;
                await _budgetRepository.UpdateAsync(currentBudget);

                var category = await _categoryRepository.GetByIdAsync(currentBudget.CategoryId);
                return MapToDto(currentBudget, category);
            }
            else
            {
                // Different month: Create new budget, mark old as completed
                var lastMonthEnd = currentMonth.AddDays(-1);

                // Mark old budget as completed
                currentBudget.EffectiveTo = lastMonthEnd;
                currentBudget.IsActive = false;
                currentBudget.UpdatedAt = DateTime.Now;
                await _budgetRepository.UpdateAsync(currentBudget);

                // Create new budget for current month
                var newBudget = new Budget(
                    Guid.NewGuid(),
                    userId,
                    currentBudget.CategoryId,
                    dto.Amount,
                    currentMonth,
                    null, // NULL = active
                    true, // IsActive
                    DateTime.Now,
                    DateTime.Now
                );

                await _budgetRepository.CreateAsync(newBudget);

                var category = await _categoryRepository.GetByIdAsync(newBudget.CategoryId);
                return MapToDto(newBudget, category);
            }
        }

        public async Task<List<BudgetDto>> GetActiveBudgetsAsync(Guid userId)
        {
            var budgets = await _budgetRepository.GetActiveBudgetsByUserAsync(userId);
            var result = new List<BudgetDto>();

            foreach (var budget in budgets)
            {
                var category = await _categoryRepository.GetByIdAsync(budget.CategoryId);
                result.Add(MapToDto(budget, category));
            }

            return result;
        }

        public async Task<List<BudgetDto>> GetBudgetHistoryAsync(Guid userId)
        {
            var budgets = await _budgetRepository.GetBudgetsByUserAsync(userId);
            var result = new List<BudgetDto>();

            foreach (var budget in budgets)
            {
                var category = await _categoryRepository.GetByIdAsync(budget.CategoryId);
                result.Add(MapToDto(budget, category));
            }

            return result;
        }

        public async Task<List<BudgetStatusDto>> GetBudgetStatusesAsync(Guid userId)
        {
            var activeBudgets = await _budgetRepository.GetActiveBudgetsByUserAsync(userId);
            var result = new List<BudgetStatusDto>();

            foreach (var budget in activeBudgets)
            {
                var category = await _categoryRepository.GetByIdAsync(budget.CategoryId);
                var status = await CalculateBudgetStatusAsync(budget, category);
                result.Add(status);
            }

            return result;
        }

        public async Task<BudgetStatusDto?> GetBudgetStatusAsync(Guid userId, Guid categoryId)
        {
            var budget = await _budgetRepository.GetActiveBudgetAsync(userId, categoryId);
            if (budget == null) return null;

            var category = await _categoryRepository.GetByIdAsync(budget.CategoryId);
            return await CalculateBudgetStatusAsync(budget, category);
        }

        public async Task DeleteBudgetAsync(Guid userId, Guid budgetId)
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new ArgumentException("Budget not found");
            }

            if (budget.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own budgets");
            }

            await _budgetRepository.DeleteAsync(budgetId);
        }

        private async Task<BudgetStatusDto> CalculateBudgetStatusAsync(Budget budget, Category? category)
        {
            var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var monthEnd = currentMonth.AddMonths(1).AddDays(-1);

            // Calculate spent amount for current month
            var spentAmount = await _transactionRepository.GetTotalSpentByCategoryAndDateRangeAsync(
                budget.UserId, budget.CategoryId, currentMonth, monthEnd);

            var remainingAmount = budget.Amount - spentAmount;
            var percentageUsed = budget.Amount > 0 ? (int)((spentAmount / budget.Amount) * 100) : 0; // Integer percentage

            string status;
            string statusColor;

            if (spentAmount > budget.Amount)
            {
                status = "Over Budget";
                statusColor = "error";
            }
            else if (percentageUsed >= 100)
            {
                status = "Budget Exhausted";
                statusColor = "error";
            }
            else if (percentageUsed >= 80)
            {
                status = "Almost There";
                statusColor = "warning";
            }
            else if (percentageUsed >= 40)
            {
                status = "On Track";
                statusColor = "success";
            }
            else if (percentageUsed >= 20)
            {
                status = "Building Momentum";
                statusColor = "info";
            }
            else if (percentageUsed >= 10)
            {
                status = "Getting Started";
                statusColor = "info";
            }
            else
            {
                status = "Fresh Start";
                statusColor = "info";
            }

            return new BudgetStatusDto
            {
                BudgetId = budget.Id,
                CategoryId = budget.CategoryId,
                CategoryName = category?.Name ?? "Unknown",
                AllocatedAmount = budget.Amount,
                SpentAmount = spentAmount,
                RemainingAmount = remainingAmount,
                PercentageUsed = percentageUsed,
                EffectiveFrom = budget.EffectiveFrom,
                EffectiveTo = budget.EffectiveTo,
                IsActive = budget.IsActive,
                Status = status,
                StatusColor = statusColor
            };
        }

        private BudgetDto MapToDto(Budget budget, Category? category)
        {
            return new BudgetDto
            {
                Id = budget.Id,
                UserId = budget.UserId,
                CategoryId = budget.CategoryId,
                Amount = budget.Amount,
                EffectiveFrom = budget.EffectiveFrom,
                EffectiveTo = budget.EffectiveTo,
                IsActive = budget.IsActive,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt,
                Category = category != null ? new CategoryDto(
                    category.Id,
                    category.UserId,
                    category.Name,
                    category.CategoryType,
                    category.Description ?? string.Empty,
                    category.CreatedAt,
                    category.UpdatedAt,
                    null
                ) : null
            };
        }
    }
}
