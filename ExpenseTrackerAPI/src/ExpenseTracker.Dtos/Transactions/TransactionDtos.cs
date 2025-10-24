using System;
using System.Collections.Generic;
using ExpenseTracker.Dtos.Accounts;
using ExpenseTracker.Dtos.Categories;
using ExpenseTracker.Dtos.Common;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Dtos.Transactions
{
    public record CreateTransactionDto(
        Guid AccountId,
        Guid CategoryId,
        Guid SubCategoryId,
        string? Description,
        decimal Amount,
        DateTime TransactionDate
    );

    public record UpdateTransactionDto(
        Guid Id,
        Guid AccountId,
        Guid CategoryId,
        Guid SubCategoryId,
        string? Description,
        decimal Amount,
        DateTime TransactionDate
    );

    public record TransactionDto(
        Guid Id,
        Guid UserId,
        Guid AccountId,
        Guid CategoryId,
        Guid? SubCategoryId,
        string Description,
        decimal Amount,
        DateTime TransactionDate,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        AccountDto Account,
        CategoryDto Category,
        SubCategoryDto? SubCategory
    );

    public record TransactionListDto(
        Guid Id,
        string Description,
        decimal Amount,
        DateTime TransactionDate,
        string AccountName,
        string CategoryName,
        string? SubCategoryName,
        CategoryType CategoryType
    );
}
