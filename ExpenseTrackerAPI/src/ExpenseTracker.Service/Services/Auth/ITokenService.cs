using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Service.Services.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashRefreshToken(string token);
    }
}
