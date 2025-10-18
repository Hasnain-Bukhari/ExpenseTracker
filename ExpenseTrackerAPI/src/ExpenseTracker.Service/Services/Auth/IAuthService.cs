using System;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Auth;

namespace ExpenseTracker.Service.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> SocialSignInAsync(SocialLoginRequest request);
        Task LogoutAsync(Guid userId, string refreshToken);
        Task<AuthResponse> RefreshAsync(RefreshRequest request);
        Task RequestPasswordResetAsync(ForgotPasswordRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}
