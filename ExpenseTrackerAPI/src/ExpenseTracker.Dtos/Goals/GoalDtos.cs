using System;

namespace ExpenseTracker.Dtos.Goals
{
    public class CreateGoalDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; } = 0;
        public Guid CategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Tag { get; set; }
        public Models.GoalStatus Status { get; set; } = Models.GoalStatus.Active;
        public Models.GoalPriority Priority { get; set; } = Models.GoalPriority.Medium;
    }

    public class UpdateGoalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Tag { get; set; }
        public Models.GoalStatus Status { get; set; }
        public Models.GoalPriority Priority { get; set; }
    }

    public class GoalDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public Guid CategoryId { get; set; }
        public ExpenseTracker.Dtos.Categories.CategoryDto? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Tag { get; set; }
        public Models.GoalStatus Status { get; set; }
        public Models.GoalPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class GoalProgressDto
    {
        public Guid Id { get; set; }
        public Guid GoalId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int PercentageComplete { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public ExpenseTracker.Dtos.Categories.CategoryDto? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Tag { get; set; }
        public Models.GoalStatus Status { get; set; }
        public Models.GoalPriority Priority { get; set; }
        public string StatusColor { get; set; } = null!;
        public string PriorityColor { get; set; } = null!;
        public int DaysRemaining { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
