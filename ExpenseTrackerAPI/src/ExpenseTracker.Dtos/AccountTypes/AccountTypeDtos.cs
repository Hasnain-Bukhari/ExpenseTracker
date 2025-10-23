using System;

namespace ExpenseTracker.Dtos.AccountTypes
{
    public record CreateAccountTypeDto(string Name, bool IsCard);
    public record UpdateAccountTypeDto(string Name, bool IsCard);
    public record AccountTypeDto(Guid Id, string Name, bool IsCard, DateTime CreatedAt, DateTime UpdatedAt);
}
