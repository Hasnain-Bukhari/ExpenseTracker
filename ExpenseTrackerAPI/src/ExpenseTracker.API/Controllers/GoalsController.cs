using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Dtos.Goals;
using ExpenseTracker.Service.Services;
using ExpenseTracker.Dtos.Common;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public GoalsController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        /// <summary>
        /// Create a new goal
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GoalDto>> CreateGoal([FromBody] CreateGoalDto createGoalDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _goalService.CreateGoalAsync(userId, createGoalDto);
                return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the goal" });
            }
        }

        /// <summary>
        /// Update an existing goal
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<GoalDto>> UpdateGoal(Guid id, [FromBody] UpdateGoalDto updateGoalDto)
        {
            try
            {
                if (id != updateGoalDto.Id)
                    return BadRequest(new { error = "Goal ID mismatch" });

                var userId = GetCurrentUserId();
                var goal = await _goalService.UpdateGoalAsync(userId, updateGoalDto);
                return Ok(goal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the goal" });
            }
        }

        /// <summary>
        /// Delete a goal
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGoal(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _goalService.DeleteGoalAsync(userId, id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the goal" });
            }
        }

        /// <summary>
        /// Get a specific goal by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GoalDto>> GetGoal(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _goalService.GetGoalByIdAsync(userId, id);
                
                if (goal == null)
                    return NotFound(new { error = "Goal not found" });

                return Ok(goal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the goal" });
            }
        }

        /// <summary>
        /// Get all goals for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetGoals()
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _goalService.GetGoalsByUserIdAsync(userId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving goals" });
            }
        }

        /// <summary>
        /// Get active goals for the current user
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetActiveGoals()
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _goalService.GetActiveGoalsByUserIdAsync(userId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving active goals" });
            }
        }

        /// <summary>
        /// Get completed goals for the current user
        /// </summary>
        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetCompletedGoals()
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _goalService.GetCompletedGoalsByUserIdAsync(userId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving completed goals" });
            }
        }

        /// <summary>
        /// Get goal progress for the current user
        /// </summary>
        [HttpGet("progress")]
        public async Task<ActionResult<IEnumerable<GoalProgressDto>>> GetGoalProgress()
        {
            try
            {
                var userId = GetCurrentUserId();
                var progress = await _goalService.GetGoalProgressByUserIdAsync(userId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving goal progress" });
            }
        }

        /// <summary>
        /// Check if a goal exists
        /// </summary>
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> GoalExists(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var exists = await _goalService.ExistsAsync(userId, id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while checking goal existence" });
            }
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
