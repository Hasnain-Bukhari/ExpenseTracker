using System;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Accounts;
using ExpenseTracker.Dtos.Currencies;

namespace ExpenseTracker.Dtos.Profile
{
    public record ProfileDto(
        Guid Id,
        string Email,
        string? FullName,
        string? Phone,
        string? ProfileImage,
        Guid? DefaultCurrencyId,
        CurrencyDto? DefaultCurrency,
        Guid? DefaultAccountId,
        AccountDto? DefaultAccount,
        string? Locale,
        string? Timezone,
        bool IsEmailVerified,
        AuthProvider Provider,
        string? ProviderId,
        DateTime? LastLoginAt,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );

    public record UpdateProfileDto(
        string? FullName,
        string? Phone,
        string? ProfileImage,
        Guid? DefaultCurrencyId,
        Guid? DefaultAccountId,
        string? Locale,
        string? Timezone
    );

    public record ChangePasswordDto(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    );

    public record ProfileImageDto(
        string ImageUrl
    );
}
