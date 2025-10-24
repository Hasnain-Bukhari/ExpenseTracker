using System;
using System.Collections.Generic;
using ExpenseTracker.Dtos.CategoryTypes;

namespace ExpenseTracker.Dtos.Categories
{
    public record CreateCategoryDto(
        string Name, 
        Guid CategoryTypeId, 
        string? Description
    );
    
    public record UpdateCategoryDto(
        Guid Id,
        string Name, 
        Guid CategoryTypeId, 
        string? Description
    );

    public record CategoryDto(
        Guid Id, 
        Guid UserId, 
        string Name, 
        Guid CategoryTypeId,
        string? Description, 
        DateTime CreatedAt, 
        DateTime UpdatedAt, 
        CategoryTypeDto CategoryType,
        List<SubCategoryDto>? SubCategories = null
    );

    public record CreateSubCategoryDto(string Name, string? Description);
    public record UpdateSubCategoryDto(string Name, string? Description);
    public record SubCategoryDto(Guid Id, Guid CategoryId, string Name, string? Description, DateTime CreatedAt, DateTime UpdatedAt);
}
