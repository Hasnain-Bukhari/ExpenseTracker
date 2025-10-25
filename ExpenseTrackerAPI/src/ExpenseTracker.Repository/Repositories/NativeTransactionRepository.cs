using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Transactions;
using ExpenseTracker.Dtos.Accounts;
using ExpenseTracker.Dtos.Categories;
using ExpenseTracker.Dtos.AccountTypes;
using ExpenseTracker.Dtos.Currencies;
using NHibernate;
using NHibernate.Linq;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeTransactionRepository : ITransactionRepository
    {
        private readonly ISessionFactory _sf;

        public NativeTransactionRepository(ISessionFactory sf)
        {
            _sf = sf;
        }

        public async Task CreateAsync(Transaction transaction)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                await s.SaveAsync(transaction);
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Transaction>()
                .Where(t => t.Id == id)
                .Fetch(t => t.Account)
                .ThenFetch(a => a.AccountType)
                .Fetch(t => t.Account)
                .ThenFetch(a => a.Currency)
                .Fetch(t => t.Category)
                .Fetch(t => t.SubCategory)
                .FirstOrDefaultAsync();
        }

        public async Task<TransactionDto?> GetByIdAsDtoAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            var transaction = await s.Query<Transaction>()
                .Where(t => t.Id == id)
                .Fetch(t => t.Account)
                .ThenFetch(a => a.AccountType)
                .Fetch(t => t.Account)
                .ThenFetch(a => a.Currency)
                .Fetch(t => t.Category)
                .Fetch(t => t.SubCategory)
                .FirstOrDefaultAsync();

            if (transaction == null) return null;

            // Create DTO within session scope
            var accountDto = new AccountDto(
                transaction.Account.Id,
                transaction.Account.UserId,
                transaction.Account.Name,
                transaction.Account.AccountTypeId,
                transaction.Account.CurrencyId,
                transaction.Account.IsSavings,
                transaction.Account.OpeningBalance,
                transaction.Account.IncludeInNetworth,
                transaction.Account.CreatedAt,
                transaction.Account.UpdatedAt,
                new AccountTypeDto(
                    transaction.Account.AccountType.Id,
                    transaction.Account.AccountType.Name,
                    transaction.Account.AccountType.IsCard,
                    transaction.Account.AccountType.CreatedAt,
                    transaction.Account.AccountType.UpdatedAt
                ),
                new CurrencyDto(
                    transaction.Account.Currency.Id,
                    transaction.Account.Currency.UserId,
                    transaction.Account.Currency.Code,
                    transaction.Account.Currency.Symbol,
                    transaction.Account.Currency.Name,
                    transaction.Account.Currency.CreatedAt,
                    transaction.Account.Currency.UpdatedAt
                )
            );

            var categoryDto = new CategoryDto(
                transaction.Category.Id,
                transaction.Category.UserId,
                transaction.Category.Name,
                transaction.Category.CategoryType,
                transaction.Category.Description,
                transaction.Category.CreatedAt,
                transaction.Category.UpdatedAt,
                null
            );

            var subCategoryDto = new SubCategoryDto(
                transaction.SubCategory.Id,
                transaction.SubCategory.CategoryId,
                transaction.SubCategory.Name,
                transaction.SubCategory.Description,
                transaction.SubCategory.CreatedAt,
                transaction.SubCategory.UpdatedAt
            );

            return new TransactionDto(
                transaction.Id,
                transaction.UserId,
                transaction.AccountId,
                transaction.CategoryId,
                transaction.SubCategoryId,
                transaction.Description ?? string.Empty,
                transaction.Amount,
                transaction.TransactionDate,
                transaction.CreatedAt,
                transaction.UpdatedAt,
                accountDto,
                categoryDto,
                subCategoryDto
            );
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                await s.UpdateAsync(transaction);
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                var transaction = await s.GetAsync<Transaction>(id);
                if (transaction != null)
                {
                    await s.DeleteAsync(transaction);
                }
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IList<Transaction>> ListByUserAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            var transactions = await s.Query<Transaction>()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            // Load navigation properties
            foreach (var transaction in transactions)
            {
                if (transaction.AccountId != Guid.Empty)
                {
                    transaction.Account = await s.GetAsync<Account>(transaction.AccountId);
                }
                if (transaction.CategoryId != Guid.Empty)
                {
                    transaction.Category = await s.GetAsync<Category>(transaction.CategoryId);
                    // CategoryType is now an enum, no need to load navigation property
                }
                if (transaction.SubCategoryId != Guid.Empty)
                {
                    transaction.SubCategory = await s.GetAsync<SubCategory>(transaction.SubCategoryId);
                }
            }

            return transactions;
        }

        public async Task<IList<Transaction>> ListByUserWithFiltersAsync(Guid userId, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            using var s = _sf.OpenSession();
            var query = s.Query<Transaction>()
                .Fetch(t => t.Account)
                .ThenFetch(a => a.AccountType)
                .Fetch(t => t.Account)
                .ThenFetch(a => a.Currency)
                .Fetch(t => t.Category)
                .Fetch(t => t.SubCategory)
                .Where(t => t.UserId == userId);

            if (accountId.HasValue)
                query = query.Where(t => t.AccountId == accountId.Value);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<long> CountByUserWithFiltersAsync(Guid userId, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate)
        {
            using var s = _sf.OpenSession();
            var query = s.Query<Transaction>()
                .Where(t => t.UserId == userId);

            if (accountId.HasValue)
                query = query.Where(t => t.AccountId == accountId.Value);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<decimal> GetTotalSpentByCategoryAndDateRangeAsync(Guid userId, Guid categoryId, DateTime startDate, DateTime endDate)
        {
            using var s = _sf.OpenSession();
            var total = await s.Query<Transaction>()
                .Where(t => t.UserId == userId && t.CategoryId == categoryId && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);
            
            return total;
        }
    }
}
