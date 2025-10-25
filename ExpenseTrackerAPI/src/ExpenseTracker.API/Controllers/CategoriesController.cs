using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Service.Services;
using ExpenseTracker.Dtos.Categories;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _service;
        public CategoriesController(CategoryService service) { _service = service; }

        private Guid GetUserId()
        {
             var sub = User.FindFirst("sub")?.Value
              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
              ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var userId))
            {
                // return 401/Forbidden to client
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? categoryType = null)
        {
            var userId = GetUserId();
            var items = await _service.ListByUserAsync(userId);
            
            if (!string.IsNullOrEmpty(categoryType))
            {
                items = items.Where(c => c.CategoryType.ToString().Equals(categoryType, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, [FromQuery] bool includeSubs = false)
        {
            var userId = GetUserId();
            var cat = await _service.GetCategoryAsync(id, userId, includeSubs);
            if (cat == null) return NotFound();
            return Ok(cat);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var userId = GetUserId();
            var created = await _service.CreateCategoryAsync(dto, userId);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var userId = GetUserId();
            try
            {
                var updated = await _service.UpdateCategoryAsync(id, dto, userId);
                return Ok(updated);
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            try
            {
                await _service.DeleteCategoryAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [Authorize]
        [HttpPost("{categoryId}/subcategories")]
        public async Task<IActionResult> CreateSub(Guid categoryId, [FromBody] CreateSubCategoryDto dto)
        {
            var userId = GetUserId();
            var created = await _service.CreateSubCategoryAsync(categoryId, dto, userId);
            return CreatedAtAction("Get", new { id = created.Id }, created);
        }

        [HttpGet("{categoryId}/subcategories")]
        public async Task<IActionResult> ListSubs(Guid categoryId)
        {
            var userId = GetUserId();
            var items = await _service.ListSubCategoriesAsync(categoryId, userId);
            return Ok(items);
        }

        [Authorize]
        [HttpPut("/v1/subcategories/{id}")]
        public async Task<IActionResult> UpdateSub(Guid id, [FromBody] UpdateSubCategoryDto dto)
        {
            var userId = GetUserId();
            try
            {
                var updated = await _service.UpdateSubCategoryAsync(id, dto, userId);
                return Ok(updated);
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [Authorize]
        [HttpDelete("/v1/subcategories/{id}")]
        public async Task<IActionResult> DeleteSub(Guid id)
        {
            var userId = GetUserId();
            try
            {
                await _service.DeleteSubCategoryAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}
