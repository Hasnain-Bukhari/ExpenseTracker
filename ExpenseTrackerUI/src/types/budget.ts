// Budget API types

export interface CreateBudgetDto {
  categoryId: string
  amount: number
}

export interface UpdateBudgetDto {
  id: string
  amount: number
}

export interface BudgetDto {
  id: string
  userId: string
  categoryId: string
  amount: number
  effectiveFrom: string
  effectiveTo?: string | null
  isActive: boolean
  createdAt: string
  updatedAt: string
  category?: CategoryDto | null
}

export interface BudgetStatusDto {
  budgetId: string
  categoryId: string
  categoryName: string
  allocatedAmount: number
  spentAmount: number
  remainingAmount: number
  percentageUsed: number
  effectiveFrom: string
  effectiveTo?: string | null
  isActive: boolean
  status: string // "On Track", "Over Budget", "Completed", "Near Limit", "Getting Started"
  statusColor: string // "success", "error", "info", "warning"
}

export interface BudgetListResponse {
  ok: boolean
  budgets: BudgetDto[]
}

export interface BudgetStatusListResponse {
  ok: boolean
  budgets: BudgetStatusDto[]
}

// Import CategoryDto from category types
import type { CategoryDto } from './category'
