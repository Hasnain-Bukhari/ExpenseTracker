using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Dtos.CategoryTypes;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Service.Services;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/category-types")]
    public class CategoryTypesController : ControllerBase
    {
        private readonly CategoryTypeService _service;

        public CategoryTypesController(CategoryTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var categoryTypes = await _service.ListActiveAsync();
                var dtos = new List<CategoryTypeDto>();
                
                foreach (var categoryType in categoryTypes)
                {
                    var dto = new CategoryTypeDto(
                        categoryType.Id,
                        categoryType.Name,
                        categoryType.Description,
                        categoryType.Color,
                        categoryType.IsActive,
                        categoryType.CreatedAt,
                        categoryType.UpdatedAt
                    );
                    dtos.Add(dto);
                }
                
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var categoryType = await _service.GetByIdAsync(id);
                if (categoryType == null)
                    return NotFound();

                var dto = new CategoryTypeDto(
                    categoryType.Id,
                    categoryType.Name,
                    categoryType.Description,
                    categoryType.Color,
                    categoryType.IsActive,
                    categoryType.CreatedAt,
                    categoryType.UpdatedAt
                );

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryTypeDto dto)
        {
            try
            {
                var entity = new CategoryType(
                    Guid.NewGuid(),
                    dto.Name,
                    dto.Description,
                    dto.Color,
                    dto.IsActive,
                    DateTime.UtcNow,
                    DateTime.UtcNow
                );

                var created = await _service.CreateAsync(entity);
                
                var outDto = new CategoryTypeDto(
                    created.Id,
                    created.Name,
                    created.Description,
                    created.Color,
                    created.IsActive,
                    created.CreatedAt,
                    created.UpdatedAt
                );

                return CreatedAtAction(nameof(Get), new { id = outDto.Id }, outDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryTypeDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            try
            {
                var entity = new CategoryType(
                    dto.Id,
                    dto.Name,
                    dto.Description,
                    dto.Color,
                    dto.IsActive,
                    DateTime.UtcNow,
                    DateTime.UtcNow
                );

                await _service.UpdateAsync(entity);
                
                var outDto = new CategoryTypeDto(
                    dto.Id,
                    dto.Name,
                    dto.Description,
                    dto.Color,
                    dto.IsActive,
                    DateTime.UtcNow,
                    DateTime.UtcNow
                );

                return Ok(outDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
