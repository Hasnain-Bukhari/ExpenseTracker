export interface CreateGoalDto {
  name: string
  description?: string
  targetAmount: number
  currentAmount?: number
  categoryId: string
  startDate: string
  endDate?: string | null
  tag?: string
  status?: GoalStatus
  priority?: GoalPriority
}

export interface UpdateGoalDto {
  id: string
  name: string
  description?: string
  targetAmount: number
  currentAmount: number
  categoryId: string
  startDate: string
  endDate?: string | null
  tag?: string
  status: GoalStatus
  priority: GoalPriority
}

export interface GoalDto {
  id: string
  userId: string
  name: string
  description?: string
  targetAmount: number
  currentAmount: number
  categoryId: string
  category?: CategoryDto
  startDate: string
  endDate?: string | null
  tag?: string
  status: GoalStatus
  priority: GoalPriority
  createdAt: string
  updatedAt: string
}

export interface GoalProgressDto {
  goalId: string
  name: string
  description?: string
  targetAmount: number
  currentAmount: number
  remainingAmount: number
  percentageComplete: number
  categoryId: string
  categoryName: string
  startDate: string
  endDate?: string | null
  tag?: string
  status: GoalStatus
  priority: GoalPriority
  statusColor: string
  priorityColor: string
  daysRemaining: number
  isOverdue: boolean
}

export interface CategoryDto {
  id: string
  name: string
  description: string
  categoryType: CategoryType
}

export enum GoalStatus {
  Active = 'Active',
  Paused = 'Paused',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum GoalPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High'
}

export enum CategoryType {
  Income = 'Income',
  Expense = 'Expense',
  TargetedSavingsGoal = 'TargetedSavingsGoal'
}

// Helper functions for UI
export const getGoalStatusLabel = (status: GoalStatus): string => {
  switch (status) {
    case GoalStatus.Active:
      return 'Active'
    case GoalStatus.Paused:
      return 'Paused'
    case GoalStatus.Completed:
      return 'Completed'
    case GoalStatus.Cancelled:
      return 'Cancelled'
    default:
      return 'Unknown'
  }
}

export const getGoalStatusColor = (status: GoalStatus): string => {
  switch (status) {
    case GoalStatus.Active:
      return 'success'
    case GoalStatus.Paused:
      return 'warning'
    case GoalStatus.Completed:
      return 'info'
    case GoalStatus.Cancelled:
      return 'error'
    default:
      return 'default'
  }
}

export const getGoalPriorityLabel = (priority: GoalPriority): string => {
  switch (priority) {
    case GoalPriority.Low:
      return 'Low'
    case GoalPriority.Medium:
      return 'Medium'
    case GoalPriority.High:
      return 'High'
    default:
      return 'Unknown'
  }
}

export const getGoalPriorityColor = (priority: GoalPriority): string => {
  switch (priority) {
    case GoalPriority.Low:
      return 'success'
    case GoalPriority.Medium:
      return 'warning'
    case GoalPriority.High:
      return 'error'
    default:
      return 'default'
  }
}

export const getGoalPriorityIcon = (priority: GoalPriority): string => {
  switch (priority) {
    case GoalPriority.Low:
      return 'mdi-arrow-down'
    case GoalPriority.Medium:
      return 'mdi-minus'
    case GoalPriority.High:
      return 'mdi-arrow-up'
    default:
      return 'mdi-help'
  }
}
