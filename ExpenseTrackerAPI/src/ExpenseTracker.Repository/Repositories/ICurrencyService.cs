using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    // Mistake: placed service interface in repository folder; will move later if requested.
    public interface ICurrencyService
    {
        Task<Currency> CreateAsync(Currency currency);
        Task UpdateAsync(Currency currency);
        Task DeleteAsync(Guid id);
        Task<Currency?> GetByIdAsync(Guid id);
        Task<IList<Currency>> ListAsync();
    }
}
