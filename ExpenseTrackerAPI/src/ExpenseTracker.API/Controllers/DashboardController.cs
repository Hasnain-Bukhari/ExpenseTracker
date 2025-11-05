using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Service.Services;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("today-spending")]
        public async Task<IActionResult> GetTodaySpending()
        {
            var userId = GetCurrentUserId();
            var spending = await _dashboardService.GetTodaySpendingAsync(userId);
            return Ok(new { spending });
        }

        [HttpGet("monthly-budget-remaining")]
        public async Task<IActionResult> GetMonthlyBudgetRemaining()
        {
            var userId = GetCurrentUserId();
            var remaining = await _dashboardService.GetMonthlyBudgetRemainingAsync(userId);
            return Ok(new { remaining });
        }

        [HttpGet("goals-progress")]
        public async Task<IActionResult> GetGoalsProgress()
        {
            var userId = GetCurrentUserId();
            var (current, target, percentage) = await _dashboardService.GetGoalsProgressSummaryAsync(userId);
            return Ok(new { current, target, percentage });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = GetCurrentUserId();
            
            var todaySpending = await _dashboardService.GetTodaySpendingAsync(userId);
            var monthlyBudgetRemaining = await _dashboardService.GetMonthlyBudgetRemainingAsync(userId);
            var (goalsCurrent, goalsTarget, goalsPercentage) = await _dashboardService.GetGoalsProgressSummaryAsync(userId);
            var spendingVsBudgetScore = await _dashboardService.GetSpendingVsBudgetScoreAsync(userId);
            
            return Ok(new
            {
                todaySpending,
                monthlyBudgetRemaining,
                goalsProgress = new
                {
                    current = goalsCurrent,
                    target = goalsTarget,
                    percentage = goalsPercentage
                },
                spendingVsBudgetScore
            });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = GetCurrentUserId();
            var (totalBalance, monthlySpend, monthlyIncome, netSavings) = await _dashboardService.GetSummaryAsync(userId);
            
            return Ok(new
            {
                totalBalance,
                monthlySpend,
                monthlyIncome,
                netSavings
            });
        }

        private Guid GetCurrentUserId()
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            return userId;
        }
    }
}

