using System;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Criterion;
using NHibernate.SqlTypes;
using NHibernate.Type;
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
            // Use direct SQL query with manual mapping to avoid NHibernate translation issues
            var sql = @"SELECT id, user_id, token_hash, expires_at, used, created_at 
                       FROM password_reset_tokens 
                       WHERE token_hash = :tokenHash 
                       LIMIT 1";
            
            var query = session.CreateSQLQuery(sql);
            query.SetParameter("tokenHash", tokenHash);
            
            var result = await query.ListAsync();
            
            if (result == null || result.Count == 0)
                return null;
            
            var row = result[0] as object[];
            if (row == null || row.Length < 6)
                return null;
            
            return new PasswordResetToken(
                (Guid)row[0],              // id
                (Guid)row[1],              // user_id
                (string)row[2],            // token_hash
                (DateTime)row[3],     // expires_at
                (bool)row[4],              // used
                (DateTime)row[5]     // created_at
            );
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
