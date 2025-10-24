using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Service.Services;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Currencies;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/currencies")]
    public class CurrenciesController : ControllerBase
    {
        private readonly CurrencyService _service;
        public CurrenciesController(CurrencyService service) { _service = service; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = GetUserId();
            var items = await _service.ListAsync(userId);
            var dtos = new System.Collections.Generic.List<CurrencyDto>();
            foreach (var c in items) dtos.Add(new CurrencyDto(c.Id, c.UserId, c.Code, c.Symbol, c.Name, c.CreatedAt, c.UpdatedAt));
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var c = await _service.GetAsync(id);
            if (c == null) return NotFound();
            return Ok(new CurrencyDto(c.Id, c.UserId, c.Code, c.Symbol, c.Name, c.CreatedAt, c.UpdatedAt));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCurrencyDto dto)
        {
            var userId = GetUserId();
            var entity = new Currency(Guid.NewGuid(), userId, dto.Code, dto.Symbol, dto.Name, DateTime.UtcNow, DateTime.UtcNow);
            await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, new CurrencyDto(entity.Id, entity.UserId, entity.Code, entity.Symbol, entity.Name, entity.CreatedAt, entity.UpdatedAt));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCurrencyDto dto)
        {
            var existing = await _service.GetAsync(id);
            if (existing == null) return NotFound();
            existing.Symbol = dto.Symbol;
            existing.Name = dto.Name;
            existing.UpdatedAt = DateTime.UtcNow;
            await _service.UpdateAsync(existing);
            return Ok(new CurrencyDto(existing.Id, existing.UserId, existing.Code, existing.Symbol, existing.Name, existing.CreatedAt, existing.UpdatedAt));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetAsync(id);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
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
