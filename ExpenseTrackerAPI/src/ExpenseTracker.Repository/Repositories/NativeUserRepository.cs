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
            
            // Use direct SQL update to ensure the changes are persisted
            var sql = @"
                UPDATE users 
                SET email = :email,
                    normalized_email = :normalizedEmail,
                    password_hash = :passwordHash,
                    full_name = :fullName,
                    default_currency_id = :defaultCurrencyId,
                    locale = :locale,
                    timezone = :timezone,
                    is_active = :isActive,
                    is_email_verified = :isEmailVerified,
                    phone = :phone,
                    profile_image = :profileImage,
                    default_account_id = :defaultAccountId,
                    provider = :provider,
                    provider_id = :providerId,
                    last_login_at = :lastLoginAt,
                    created_at = :createdAt,
                    updated_at = :updatedAt
                WHERE id = :id";
            
            await session.CreateSQLQuery(sql)
                .SetParameter("email", user.Email)
                .SetParameter("normalizedEmail", user.NormalizedEmail)
                .SetParameter("passwordHash", user.PasswordHash)
                .SetParameter("fullName", user.FullName)
                .SetParameter("defaultCurrencyId", user.DefaultCurrencyId)
                .SetParameter("locale", user.Locale)
                .SetParameter("timezone", user.Timezone)
                .SetParameter("isActive", user.IsActive)
                .SetParameter("isEmailVerified", user.IsEmailVerified)
                .SetParameter("phone", user.Phone)
                .SetParameter("profileImage", user.ProfileImage)
                .SetParameter("defaultAccountId", user.DefaultAccountId)
                .SetParameter("provider", user.Provider.ToString())
                .SetParameter("providerId", user.ProviderId)
                .SetParameter("lastLoginAt", user.LastLoginAt)
                .SetParameter("createdAt", user.CreatedAt)
                .SetParameter("updatedAt", user.UpdatedAt)
                .SetParameter("id", user.Id)
                .ExecuteUpdateAsync();
            
            await tx.CommitAsync();
        }
    }
}
