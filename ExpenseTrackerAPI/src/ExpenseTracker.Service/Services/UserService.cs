using System;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task CreateUserAsync(User user);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User?> GetUserAsync(Guid id) => _userRepository.GetAsync(id);

        public Task<User?> GetByEmailAsync(string email) => _userRepository.FindByEmailAsync(email);

        public Task CreateUserAsync(User user) => _userRepository.SaveAsync(user);
    }
}
