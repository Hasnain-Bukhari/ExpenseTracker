using System;

namespace ExpenseTracker.Dtos.CategoryTypes
{
    public record CreateCategoryTypeDto(
        string Name,
        string? Description,
        string? Color,
        bool IsActive
    );

    public record UpdateCategoryTypeDto(
        Guid Id,
        string Name,
        string? Description,
        string? Color,
        bool IsActive
    );

    public record CategoryTypeDto(
        Guid Id,
        string Name,
        string? Description,
        string? Color,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
