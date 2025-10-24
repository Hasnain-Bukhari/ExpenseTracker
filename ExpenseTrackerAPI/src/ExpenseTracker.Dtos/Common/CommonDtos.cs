using System;
using System.Collections.Generic;

namespace ExpenseTracker.Dtos.Common
{
    public record PagedResult<T>(
        IList<T> Items, 
        int Page, 
        int PageSize, 
        long Total
    );

    public record BaseEntityDto(
        Guid Id,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
