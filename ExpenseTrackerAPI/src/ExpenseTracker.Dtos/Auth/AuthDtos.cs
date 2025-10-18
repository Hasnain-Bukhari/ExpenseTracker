using System;

namespace ExpenseTracker.Dtos.Auth
{
    public record RegisterRequest(
        string Name,
        string Email,
        string? Phone,
        string? Password,
        bool AcceptTerms
    );

    public record LoginRequest(
        string Email,
        string Password,
        bool Remember
    );

    public record SocialLoginRequest(
        string Provider,
        string Token,
        bool Remember
    );

    public record RefreshRequest(string RefreshToken);

    public record ForgotPasswordRequest(string Email);

    public record ResetPasswordRequest(string Token, string NewPassword);

    public record AuthUserDto(Guid Id, string Name, string Email);

    public record AuthResponse(
        bool Ok,
        AuthUserDto User,
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
