using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _repo;
        private readonly IAccountTypeRepository _typeRepo;
        public AccountService(IAccountRepository repo, IAccountTypeRepository typeRepo)
        {
            _repo = repo;
            _typeRepo = typeRepo;
        }

        public async Task<Account> CreateAsync(Account a)
        {
            // Validate account type exists
            var at = await _typeRepo.GetByIdAsync(a.AccountTypeId);
            if (at == null) throw new InvalidOperationException("Account type not found");

            // If account type is a card, we won't accept CVV; ensure DTO didn't include it (API should not send CVV)
            if (at.IsCard)
            {
                // nothing to enforce server-side as we don't store CVV; rely on DTO contract
            }

            a.Id = Guid.NewGuid();
            a.CreatedAt = DateTime.UtcNow;
            a.UpdatedAt = DateTime.UtcNow;
            await _repo.CreateAsync(a);
            return a;
        }

        public async Task UpdateAsync(Account a)
        {
            var existing = await _repo.GetByIdAsync(a.Id);
            if (existing == null) throw new InvalidOperationException("Account not found");

            var at = await _typeRepo.GetByIdAsync(a.AccountTypeId);
            if (at == null) throw new InvalidOperationException("Account type not found");

            existing.Name = a.Name;
            existing.AccountTypeId = a.AccountTypeId;
            existing.CurrencyId = a.CurrencyId;
            existing.IsSavings = a.IsSavings;
            existing.OpeningBalance = a.OpeningBalance;
            existing.IncludeInNetworth = a.IncludeInNetworth;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(existing);
        }

        public async Task<IList<Account>> ListAsync(Guid userId)
        {
            return await _repo.ListByUserIdAsync(userId);
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
