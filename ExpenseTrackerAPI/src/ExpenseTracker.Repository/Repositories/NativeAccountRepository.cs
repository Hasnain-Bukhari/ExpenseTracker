using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface IAccountRepository
    {
        Task CreateAsync(Account a);
        Task<Account?> GetByIdAsync(Guid id);
        Task UpdateAsync(Account a);
        Task DeleteAsync(Guid id);
        Task<IList<Account>> ListByUserIdAsync(Guid userId);
    }

    public class NativeAccountRepository : IAccountRepository
    {
        private readonly ISessionFactory _sf;
        public NativeAccountRepository(ISessionFactory sf) { _sf = sf; }

        public async Task CreateAsync(Account a)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.SaveAsync(a);
            await tx.CommitAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.GetAsync<Account>(id) as Account;
        }

        public async Task UpdateAsync(Account a)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.UpdateAsync(a);
            await tx.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            var o = await s.GetAsync<Account>(id);
            if (o != null) await s.DeleteAsync(o);
            await tx.CommitAsync();
        }

        public async Task<IList<Account>> ListByUserIdAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            var accounts = await s.Query<Account>()
                .Where(a => a.UserId == userId)
                .ToListAsync();
            
            // Load navigation properties
            foreach (var account in accounts)
            {
                if (account.AccountTypeId != Guid.Empty)
                {
                    account.AccountType = await s.GetAsync<AccountType>(account.AccountTypeId);
                }
                if (account.CurrencyId != Guid.Empty)
                {
                    account.Currency = await s.GetAsync<Currency>(account.CurrencyId);
                }
            }
            
            return accounts;
        }
    }
}
