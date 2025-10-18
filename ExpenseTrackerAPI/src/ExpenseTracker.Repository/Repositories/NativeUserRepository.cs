using System;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeUserRepository : IUserRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public NativeUserRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task CreateAsync(User user)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            await session.SaveAsync(user);
            await tx.CommitAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try {
                using var session = _sessionFactory.OpenSession();
                return await session.Query<User>().FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to get user by email '{email}': {ex}");
                throw ex;
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using var session = _sessionFactory.OpenSession();
            return await session.GetAsync<User>(id);
        }

        public async Task<User?> GetByProviderAsync(string provider, string providerId)
        {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<User>().FirstOrDefaultAsync(u => u.Provider.ToString() == provider && u.ProviderId == providerId);
        }

        public async Task UpdateAsync(User user)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            await session.UpdateAsync(user);
            await tx.CommitAsync();
        }
    }
}
