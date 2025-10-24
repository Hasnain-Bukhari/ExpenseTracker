using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public class CurrencyService
    {
        private readonly ICurrencyRepository _repo;
        public CurrencyService(ICurrencyRepository repo) { _repo = repo; }

        public Task<IList<Currency>> ListAsync(Guid userId) => _repo.ListByUserIdAsync(userId);
        public Task<Currency?> GetAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task CreateAsync(Currency c) => _repo.CreateAsync(c);
        public Task UpdateAsync(Currency c) => _repo.UpdateAsync(c);
        public Task DeleteAsync(Guid id) => _repo.DeleteAsync(id);
    }
}
