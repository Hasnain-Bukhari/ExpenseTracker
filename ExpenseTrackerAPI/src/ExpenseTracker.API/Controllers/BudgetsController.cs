using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Dtos.Budgets;
using ExpenseTracker.Service.Services;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly BudgetService _budgetService;

        public BudgetsController(BudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto dto)
        {
            try
            {
                var userId = GetUserId();
                var budget = await _budgetService.CreateBudgetAsync(userId, dto);
                return Ok(new { ok = true, budget });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ok = false, error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { ok = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = "An error occurred while creating the budget", details = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetDto dto)
        {
            try
            {
                dto.Id = id; // Ensure the ID matches the route parameter
                var userId = GetUserId();
                var budget = await _budgetService.UpdateBudgetAsync(userId, dto);
                return Ok(new { ok = true, budget });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ok = false, error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { ok = false, error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = "An error occurred while updating the budget" });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBudgets()
        {
            try
            {
                var userId = GetUserId();
                var budgets = await _budgetService.GetActiveBudgetsAsync(userId);
                return Ok(new { ok = true, budgets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = "An error occurred while retrieving active budgets" });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetBudgetHistory()
        {
            try
            {
                var userId = GetUserId();
                var budgets = await _budgetService.GetBudgetHistoryAsync(userId);
                return Ok(new { ok = true, budgets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = "An error occurred while retrieving budget history" });
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetBudgetStatuses()
        {
            try
            {
                var userId = GetUserId();
                var statuses = await _budgetService.GetBudgetStatusesAsync(userId);
                return Ok(new { ok = true, budgets = statuses });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = "An error occurred while retrieving budget statuses" });
            }
        }

        [HttpGet("status/{categoryId}")]
        public async Task<IActionResult> GetBudgetStatus(Guid categoryId)
        {
            try
            {
                var userId = GetUserId();
                var status = await _budgetService.GetBudgetStatusAsync(userId, categoryId);
                if (status == null)
                {
                    return NotFound(new { ok = false, error = "No budget found for this category" });
                }
                return Ok(new { ok = true, budget = status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = "An error occurred while retrieving budget status" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            try
            {
                var userId = GetUserId();
                await _budgetService.DeleteBudgetAsync(userId, id);
                return Ok(new { ok = true, message = "Budget deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ok = false, error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = "An error occurred while deleting the budget" });
            }
        }

        private Guid GetUserId()
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
