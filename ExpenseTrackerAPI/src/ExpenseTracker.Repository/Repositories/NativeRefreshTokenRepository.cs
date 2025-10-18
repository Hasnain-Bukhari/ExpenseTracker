using System;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public NativeRefreshTokenRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task CreateAsync(RefreshToken token)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            await session.SaveAsync(token);
            await tx.CommitAsync();
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<RefreshToken>().FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task RevokeAsync(Guid id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            var token = await session.GetAsync<RefreshToken>(id);
            if (token != null)
            {
                token = token with { RevokedAt = DateTimeOffset.UtcNow };
                await session.UpdateAsync(token);
            }
            await tx.CommitAsync();
        }

        public async Task RotateAsync(Guid id, RefreshToken newToken)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            var old = await session.GetAsync<RefreshToken>(id);
            if (old != null)
            {
                old = old with { RevokedAt = DateTimeOffset.UtcNow };
                await session.UpdateAsync(old);
            }
            await session.SaveAsync(newToken);
            await tx.CommitAsync();
        }
    }
}
