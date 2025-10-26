using System;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Repositories;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Service.Services
{
    public class DashboardService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IGoalRepository _goalRepository;

        public DashboardService(
            ITransactionRepository transactionRepository,
            IBudgetRepository budgetRepository,
            IGoalRepository goalRepository)
        {
            _transactionRepository = transactionRepository;
            _budgetRepository = budgetRepository;
            _goalRepository = goalRepository;
        }

        public async Task<decimal> GetTodaySpendingAsync(Guid userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            return await _transactionRepository.GetTotalSpendingByDateRangeAsync(userId, today, tomorrow);
        }

        public async Task<decimal> GetMonthlyBudgetRemainingAsync(Guid userId)
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var tomorrow = DateTime.Today.AddDays(1);

            // Get all active budgets
            var budgets = await _budgetRepository.GetActiveBudgetsByUserAsync(userId);
            
            if (budgets == null || !budgets.Any())
            {
                return 0; // No budgets configured
            }

            decimal totalBudget = 0;
            decimal totalSpent = 0;

            foreach (var budget in budgets)
            {
                totalBudget += budget.Amount;
                
                // Calculate spending for this category in current month
                var spending = await _transactionRepository.GetTotalSpentByCategoryAndDateRangeAsync(
                    userId, 
                    budget.CategoryId, 
                    startOfMonth, 
                    tomorrow);
                    
                totalSpent += spending;
            }

            return Math.Max(0, totalBudget - totalSpent);
        }

        public async Task<(decimal Current, decimal Target, decimal Percentage)> GetGoalsProgressSummaryAsync(Guid userId)
        {
            var goals = await _goalRepository.GetActiveGoalsByUserIdAsync(userId);
            
            if (goals == null || !goals.Any())
            {
                return (0, 0, 0);
            }

            decimal currentTotal = 0;
            decimal targetTotal = 0;

            foreach (var goal in goals)
            {
                // Calculate current amount from transactions for this goal's category
                decimal actualCurrentAmount = await _transactionRepository.GetTotalSpentByCategoryAndDateRangeAsync(
                    userId,
                    goal.CategoryId,
                    goal.StartDate,
                    DateTime.UtcNow);

                // Use the calculated amount or the goal's current amount, whichever is higher
                // This allows for initial amounts set on the goal
                decimal currentAmount = actualCurrentAmount > 0 ? actualCurrentAmount : goal.CurrentAmount;
                
                currentTotal += currentAmount;
                targetTotal += goal.TargetAmount;
            }

            decimal percentage = targetTotal > 0 ? (currentTotal / targetTotal) * 100 : 0;

            return (currentTotal, targetTotal, percentage);
        }

        public async Task<int> GetSpendingVsBudgetScoreAsync(Guid userId)
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var tomorrow = DateTime.Today.AddDays(1);

            // Get all active budgets
            var budgets = await _budgetRepository.GetActiveBudgetsByUserAsync(userId);
            
            if (budgets == null || !budgets.Any())
            {
                return 100; // Perfect score if no budgets configured
            }

            decimal totalBudget = 0;
            decimal totalSpent = 0;

            foreach (var budget in budgets)
            {
                totalBudget += budget.Amount;
                
                // Calculate spending for this category in current month
                var spending = await _transactionRepository.GetTotalSpentByCategoryAndDateRangeAsync(
                    userId, 
                    budget.CategoryId, 
                    startOfMonth, 
                    tomorrow);
                    
                totalSpent += spending;
            }

            if (totalBudget == 0)
            {
                return 100; // Perfect score if budget is 0
            }

            // Calculate percentage: (remaining budget / total budget) * 100
            // Score ranges from 0 (over budget) to 100 (at or under budget)
            decimal score = (totalBudget - totalSpent) / totalBudget * 100;
            return Math.Max(0, Math.Min(100, (int)score));
        }
    }
}

