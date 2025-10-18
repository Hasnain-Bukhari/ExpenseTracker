using System;

namespace ExpenseTracker.Dtos.Models
{
    public enum AccountType { Cash, Bank, CreditCard, Savings, Other }
    public enum CategoryType { Expense, Income }
    public enum TransactionType { Expense, Income, Transfer, Saving }
    public enum TransactionStatus { Pending, Cleared, Reconciled }
    public enum BudgetPeriod { Monthly, Weekly, Yearly, Custom }

    public record User(
        Guid Id,
        string Email,
        string PasswordHash,
        string? FullName,
        string DefaultCurrency,
        string? Locale,
        string? Timezone,
        bool IsActive,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );

    public record Currency(
        string Code,
        string? Symbol,
        string? Name
    );

    public record Account(
        Guid Id,
        Guid UserId,
        string Name,
        AccountType Type,
        string Currency,
        bool IsSavings,
        decimal OpeningBalance,
        bool IncludeInNetworth,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );

    public record Category(
        Guid Id,
        Guid UserId,
        string Name,
        Guid? ParentId,
        CategoryType Type,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );

    public record Goal(
        Guid Id,
        Guid UserId,
        string Name,
        decimal TargetAmount,
        string Currency,
        Guid? AccountId,
        DateTime? Deadline,
        string? Note,
        bool Archived,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );

    public record Transaction(
        Guid Id,
        Guid UserId,
        Guid AccountId,
        TransactionType Type,
        decimal Amount,
        string Currency,
        decimal? OriginalAmount,
        string? OriginalCurrency,
        DateTime Date,
        DateTimeOffset? SettledAt,
        Guid? CategoryId,
        Guid? GoalId,
        string? Notes,
        TransactionStatus Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );

    public record Budget(
        Guid Id,
        Guid UserId,
        Guid CategoryId,
        BudgetPeriod Period,
        decimal Amount,
        DateTime StartDate,
        DateTime? EndDate,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );
}
