using System;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByProviderAsync(string provider, string providerId);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
    }
}
