using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeAccountTypeRepository : IAccountTypeRepository
    {
        private readonly ISessionFactory _sf;
        public NativeAccountTypeRepository(ISessionFactory sf) { _sf = sf; }

        public async Task CreateAsync(AccountType accountType)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.SaveAsync(accountType);
            await tx.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            var o = await s.GetAsync<AccountType>(id);
            if (o != null) await s.DeleteAsync(o);
            await tx.CommitAsync();
        }

        public async Task<AccountType?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.GetAsync<AccountType>(id) as AccountType;
        }

        public async Task<AccountType?> GetByNameAsync(string name)
        {
            using var s = _sf.OpenSession();
            return await s.Query<AccountType>().FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
        }

        public async Task<IList<AccountType>> ListAsync()
        {
            using var s = _sf.OpenSession();
            return await s.Query<AccountType>().ToListAsync();
        }

        public async Task<IList<AccountType>> ListByUserIdAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<AccountType>()
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(AccountType accountType)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.UpdateAsync(accountType);
            await tx.CommitAsync();
        }
    }
}
