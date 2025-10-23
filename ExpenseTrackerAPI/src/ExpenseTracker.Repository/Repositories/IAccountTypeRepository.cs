using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface IAccountTypeRepository
    {
        Task<AccountType?> GetByIdAsync(Guid id);
        Task<AccountType?> GetByNameAsync(string name);
        Task CreateAsync(AccountType accountType);
        Task UpdateAsync(AccountType accountType);
        Task DeleteAsync(Guid id);
        Task<IList<AccountType>> ListAsync();
    }
}
