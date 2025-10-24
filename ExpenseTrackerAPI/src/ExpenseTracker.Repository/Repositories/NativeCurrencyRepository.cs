using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeCurrencyRepository : ICurrencyRepository
    {
        private readonly ISessionFactory _sf;
        public NativeCurrencyRepository(ISessionFactory sf) { _sf = sf; }

        public async Task CreateAsync(Currency currency)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.SaveAsync(currency);
            await tx.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            var o = await s.GetAsync<Currency>(id);
            if (o != null) await s.DeleteAsync(o);
            await tx.CommitAsync();
        }

        public async Task<Currency?> GetByCodeAsync(string code)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Currency>().FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<Currency?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.GetAsync<Currency>(id) as Currency;
        }

        public async Task<IList<Currency>> ListAsync()
        {
            using var s = _sf.OpenSession();
            return await s.Query<Currency>().ToListAsync();
        }

        public async Task<IList<Currency>> ListByUserIdAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Currency>()
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Currency currency)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.UpdateAsync(currency);
            await tx.CommitAsync();
        }
    }
}
