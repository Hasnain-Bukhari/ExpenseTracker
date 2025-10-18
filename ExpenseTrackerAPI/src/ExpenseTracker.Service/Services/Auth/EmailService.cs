using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Service.Services.Auth
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(User user);
        Task SendPasswordResetAsync(User user, string token);
    }

    public class ConsoleEmailService : IEmailService
    {
        public Task SendWelcomeEmailAsync(User user)
        {
            System.Console.WriteLine($"[Email] Welcome {user.Email}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(User user, string token)
        {
            System.Console.WriteLine($"[Email] Password reset for {user.Email}: token={token}");
            return Task.CompletedTask;
        }
    }
}
