using System;

namespace ExpenseTracker.Dtos.Currencies
{
    public record CreateCurrencyDto(string Code, string? Symbol, string? Name);
    public record UpdateCurrencyDto(string? Symbol, string? Name);
    public record CurrencyDto(Guid Id, Guid? UserId, string Code, string? Symbol, string? Name, DateTime CreatedAt, DateTime UpdatedAt);
}
