using System;
using ExpenseTracker.Dtos.Categories;

namespace ExpenseTracker.Dtos.Budgets
{
    public class CreateBudgetDto
    {
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
    }

    public class UpdateBudgetDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
    }

    public class BudgetDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        
        // Navigation properties
        public CategoryDto? Category { get; set; }
    }

    public class BudgetStatusDto
    {
        public Guid BudgetId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal AllocatedAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int PercentageUsed { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = null!; // "On Track", "Over Budget", "Completed"
        public string StatusColor { get; set; } = null!; // "success", "error", "info"
    }

    public class BudgetListResponse
    {
        public bool Ok { get; set; }
        public List<BudgetDto> Budgets { get; set; } = new List<BudgetDto>();
    }

    public class BudgetStatusListResponse
    {
        public bool Ok { get; set; }
        public List<BudgetStatusDto> Budgets { get; set; } = new List<BudgetStatusDto>();
    }
}
