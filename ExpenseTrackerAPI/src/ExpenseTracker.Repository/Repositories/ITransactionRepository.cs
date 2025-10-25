using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Transactions;

namespace ExpenseTracker.Repository.Repositories
{
    public interface ITransactionRepository
    {
        Task CreateAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<TransactionDto?> GetByIdAsDtoAsync(Guid id);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Guid id);
        Task<IList<Transaction>> ListByUserAsync(Guid userId);
        Task<IList<Transaction>> ListByUserWithFiltersAsync(Guid userId, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<long> CountByUserWithFiltersAsync(Guid userId, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate);
        Task<decimal> GetTotalSpentByCategoryAndDateRangeAsync(Guid userId, Guid categoryId, DateTime startDate, DateTime endDate);
    }
}
