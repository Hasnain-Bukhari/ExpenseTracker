using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Service.Services;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.AccountTypes;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/account-types")]
    public class AccountTypesController : ControllerBase
    {
        private readonly AccountTypeService _service;
        public AccountTypesController(AccountTypeService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var items = await _service.ListAsync();
            var dtos = new System.Collections.Generic.List<AccountTypeDto>();
            foreach (var a in items) dtos.Add(new AccountTypeDto(a.Id, a.Name, a.IsCard, a.CreatedAt, a.UpdatedAt));
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var c = await _service.GetAsync(id);
            if (c == null) return NotFound();
            return Ok(new AccountTypeDto(c.Id, c.Name, c.IsCard, c.CreatedAt, c.UpdatedAt));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountTypeDto dto)
        {
            var entity = new AccountType(Guid.NewGuid(), dto.Name, dto.IsCard, DateTime.UtcNow, DateTime.UtcNow);
            await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, new AccountTypeDto(entity.Id, entity.Name, entity.IsCard, entity.CreatedAt, entity.UpdatedAt));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountTypeDto dto)
        {
            var existing = await _service.GetAsync(id);
            if (existing == null) return NotFound();
            existing.Name = dto.Name;
            existing.IsCard = dto.IsCard;
            existing.UpdatedAt = DateTime.UtcNow;
            await _service.UpdateAsync(existing);
            return Ok(new AccountTypeDto(existing.Id, existing.Name, existing.IsCard, existing.CreatedAt, existing.UpdatedAt));
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
    }
}
