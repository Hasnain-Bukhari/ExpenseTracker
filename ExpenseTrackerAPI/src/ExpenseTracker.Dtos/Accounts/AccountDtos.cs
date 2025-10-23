using System;
using ExpenseTracker.Dtos.AccountTypes;
using ExpenseTracker.Dtos.Currencies;

namespace ExpenseTracker.Dtos.Accounts
{
    public record CreateAccountDto(
        string Name,
        Guid AccountTypeId,
        Guid CurrencyId,
        bool IsSavings,
        decimal OpeningBalance,
        bool IncludeInNetworth
    );

    public record UpdateAccountDto(
        Guid Id,
        string Name,
        Guid AccountTypeId,
        Guid CurrencyId,
        bool IsSavings,
        decimal OpeningBalance,
        bool IncludeInNetworth
    );

    public record AccountDto(
        Guid Id,
        Guid UserId,
        string Name,
        Guid AccountTypeId,
        Guid CurrencyId,
        bool IsSavings,
        decimal OpeningBalance,
        bool IncludeInNetworth,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        AccountTypeDto AccountType,
        CurrencyDto Currency
    );
}