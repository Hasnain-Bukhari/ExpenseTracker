import { api } from '@/lib/api'

export interface DashboardStats {
  todaySpending: number
  monthlyBudgetRemaining: number
  goalsProgress: {
    current: number
    target: number
    percentage: number
  }
  spendingVsBudgetScore: number
}

export interface DashboardSummary {
  totalBalance: number
  monthlySpend: number
  monthlyIncome: number
  netSavings: number
}

export const dashboardService = {
  async getStats(): Promise<DashboardStats> {
    const response = await api.get<DashboardStats>('/dashboard/stats')
    return response.data
  },

  async getTodaySpending(): Promise<number> {
    const response = await api.get<{ spending: number }>('/dashboard/today-spending')
    return response.data.spending
  },

  async getMonthlyBudgetRemaining(): Promise<number> {
    const response = await api.get<{ remaining: number }>('/dashboard/monthly-budget-remaining')
    return response.data.remaining
  },

  async getGoalsProgress(): Promise<{ current: number; target: number; percentage: number }> {
    const response = await api.get<{ current: number; target: number; percentage: number }>('/dashboard/goals-progress')
    return response.data
  },

  async getSummary(): Promise<DashboardSummary> {
    const response = await api.get<DashboardSummary>('/dashboard/summary')
    return response.data
  }
}

