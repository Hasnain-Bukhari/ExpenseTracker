using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public class AccountTypeService
    {
        private readonly IAccountTypeRepository _repo;
        public AccountTypeService(IAccountTypeRepository repo) { _repo = repo; }

        public Task<IList<AccountType>> ListAsync(Guid userId) => _repo.ListByUserIdAsync(userId);
        public Task<AccountType?> GetAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task CreateAsync(AccountType a) => _repo.CreateAsync(a);
        public Task UpdateAsync(AccountType a) => _repo.UpdateAsync(a);
        public Task DeleteAsync(Guid id) => _repo.DeleteAsync(id);
    }
}
