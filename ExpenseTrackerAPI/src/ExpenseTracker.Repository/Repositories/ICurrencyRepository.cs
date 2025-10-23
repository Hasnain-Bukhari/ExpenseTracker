using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetByIdAsync(Guid id);
        Task<Currency?> GetByCodeAsync(string code);
        Task CreateAsync(Currency currency);
        Task UpdateAsync(Currency currency);
        Task DeleteAsync(Guid id);
        Task<IList<Currency>> ListAsync();
    }
}
