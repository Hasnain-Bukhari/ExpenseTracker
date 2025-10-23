using System;
using System.Collections.Generic;

namespace ExpenseTracker.Dtos.Categories
{
    public enum CategoryTypeDto { expense, income }

    public record CreateCategoryDto(string Name, string Type, string? Description);
    public record UpdateCategoryDto(string Name, string? Description);

    public record CategoryDto(Guid Id, Guid UserId, string Name, string Type, string? Description, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, List<SubCategoryDto>? SubCategories = null);

    public record CreateSubCategoryDto(string Name, string? Description);
    public record UpdateSubCategoryDto(string Name, string? Description);
    public record SubCategoryDto(Guid Id, Guid CategoryId, string Name, string? Description, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

    public record PagedResult<T>(IList<T> Items, int Page, int PageSize, long Total);
}
