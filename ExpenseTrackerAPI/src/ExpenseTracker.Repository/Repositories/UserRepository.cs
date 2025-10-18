using System;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<User?> FindByEmailAsync(string email);
    }
}
