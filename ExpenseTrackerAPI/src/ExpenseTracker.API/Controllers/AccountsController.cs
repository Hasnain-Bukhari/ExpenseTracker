using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Service.Services;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Accounts;
using ExpenseTracker.Dtos.AccountTypes;
using ExpenseTracker.Dtos.Currencies;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _service;
        private readonly IAccountTypeRepository _accountTypeRepo;
        private readonly ICurrencyRepository _currencyRepo;
        
        public AccountsController(AccountService service, IAccountTypeRepository accountTypeRepo, ICurrencyRepository currencyRepo) 
        { 
            _service = service;
            _accountTypeRepo = accountTypeRepo;
            _currencyRepo = currencyRepo;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var userId = GetUserId();
                var accounts = await _service.ListAsync(userId);
                var dtos = new List<AccountDto>();
                
                foreach (var account in accounts)
                {
                    var accountTypeDto = new AccountTypeDto(account.AccountType!.Id, account.AccountType.Name, account.AccountType.IsCard, account.AccountType.CreatedAt, account.AccountType.UpdatedAt);
                    var currencyDto = new CurrencyDto(account.Currency!.Id, account.Currency.UserId, account.Currency.Code, account.Currency.Symbol, account.Currency.Name, account.Currency.CreatedAt, account.Currency.UpdatedAt);
                    var dto = new AccountDto(account.Id, account.UserId, account.Name, account.AccountType.Id, account.Currency.Id, account.IsSavings, account.OpeningBalance, account.IncludeInNetworth, account.CreatedAt, account.UpdatedAt, accountTypeDto, currencyDto);
                    dtos.Add(dto);
                }
                
                return Ok(dtos);
            }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
        {
            try
            {
                var userId = GetUserId();
                
                // Validate AccountType and Currency exist
                var accountType = await _accountTypeRepo.GetByIdAsync(dto.AccountTypeId);
                if (accountType == null) return BadRequest(new { error = "Account type not found" });
                
                var currency = await _currencyRepo.GetByIdAsync(dto.CurrencyId);
                if (currency == null) return BadRequest(new { error = "Currency not found" });
                
                var entity = new Account(Guid.NewGuid(), userId, dto.Name, dto.AccountTypeId, dto.CurrencyId, dto.IsSavings, dto.OpeningBalance, dto.IncludeInNetworth, DateTime.UtcNow, DateTime.UtcNow);
                var created = await _service.CreateAsync(entity);
                
                var accountTypeDto = new AccountTypeDto(accountType.Id, accountType.Name, accountType.IsCard, accountType.CreatedAt, accountType.UpdatedAt);
                var currencyDto = new CurrencyDto(currency.Id, currency.UserId, currency.Code, currency.Symbol, currency.Name, currency.CreatedAt, currency.UpdatedAt);
                var outDto = new AccountDto(created.Id, created.UserId, created.Name, created.AccountTypeId, created.CurrencyId, created.IsSavings, created.OpeningBalance, created.IncludeInNetworth, created.CreatedAt, created.UpdatedAt, accountTypeDto, currencyDto);
                
                return CreatedAtAction(nameof(Get), new { id = outDto.Id }, outDto);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountDto dto)
        {
            if (id != dto.Id) return BadRequest();
            try
            {
                var userId = GetUserId();
                
                // Validate AccountType and Currency exist
                var accountType = await _accountTypeRepo.GetByIdAsync(dto.AccountTypeId);
                if (accountType == null) return BadRequest(new { error = "Account type not found" });
                
                var currency = await _currencyRepo.GetByIdAsync(dto.CurrencyId);
                if (currency == null) return BadRequest(new { error = "Currency not found" });
                
                var entity = new Account(dto.Id, userId, dto.Name, dto.AccountTypeId, dto.CurrencyId, dto.IsSavings, dto.OpeningBalance, dto.IncludeInNetworth, DateTime.UtcNow, DateTime.UtcNow);
                await _service.UpdateAsync(entity);
                
                var accountTypeDto = new AccountTypeDto(accountType.Id, accountType.Name, accountType.IsCard, accountType.CreatedAt, accountType.UpdatedAt);
                var currencyDto = new CurrencyDto(currency.Id, currency.UserId, currency.Code, currency.Symbol, currency.Name, currency.CreatedAt, currency.UpdatedAt);
                var outDto = new AccountDto(dto.Id, userId, dto.Name, dto.AccountTypeId, dto.CurrencyId, dto.IsSavings, dto.OpeningBalance, dto.IncludeInNetworth, DateTime.UtcNow, DateTime.UtcNow, accountTypeDto, currencyDto);
                
                return Ok(outDto);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var a = await _service.GetByIdAsync(id);
            if (a == null) return NotFound();
            
            var accountTypeDto = new AccountTypeDto(a.AccountType!.Id, a.AccountType.Name, a.AccountType.IsCard, a.AccountType.CreatedAt, a.AccountType.UpdatedAt);
            var currencyDto = new CurrencyDto(a.Currency!.Id, a.Currency.UserId, a.Currency.Code, a.Currency.Symbol, a.Currency.Name, a.Currency.CreatedAt, a.Currency.UpdatedAt);
            var dto = new AccountDto(a.Id, a.UserId, a.Name, a.AccountType.Id, a.Currency.Id, a.IsSavings, a.OpeningBalance, a.IncludeInNetworth, a.CreatedAt, a.UpdatedAt, accountTypeDto, currencyDto);
            
            return Ok(dto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
