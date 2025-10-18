using System;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativePasswordResetRepository : IPasswordResetRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public NativePasswordResetRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task CreateAsync(PasswordResetToken token)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            await session.SaveAsync(token);
            await tx.CommitAsync();
        }

        public async Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash)
        {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<PasswordResetToken>().FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task MarkUsedAsync(Guid id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            var token = await session.GetAsync<PasswordResetToken>(id);
            if (token != null)
            {
                token.Used = true;
                await session.UpdateAsync(token);
            }
            await tx.CommitAsync();
        }
    }
}
