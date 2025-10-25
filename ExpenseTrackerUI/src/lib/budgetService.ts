import { api } from '@/lib/api'
import type { 
  CreateBudgetDto, 
  UpdateBudgetDto, 
  BudgetDto, 
  BudgetStatusDto,
  BudgetListResponse,
  BudgetStatusListResponse 
} from '@/types/budget'

export const budgetService = {
  // Create a new budget
  async create(data: CreateBudgetDto): Promise<BudgetDto> {
    const response = await api.post('/budgets', data)
    return response.data.budget
  },

  // Update an existing budget
  async update(id: string, data: UpdateBudgetDto): Promise<BudgetDto> {
    const response = await api.put(`/budgets/${id}`, data)
    return response.data.budget
  },

  // Get active budgets
  async getActive(): Promise<BudgetDto[]> {
    const response = await api.get<BudgetListResponse>('/budgets/active')
    return response.data.budgets
  },

  // Get budget history
  async getHistory(): Promise<BudgetDto[]> {
    const response = await api.get<BudgetListResponse>('/budgets/history')
    return response.data.budgets
  },

  // Get budget statuses
  async getStatuses(): Promise<BudgetStatusDto[]> {
    const response = await api.get<BudgetStatusListResponse>('/budgets/status')
    return response.data.budgets
  },

  // Get budget status for specific category
  async getStatus(categoryId: string): Promise<BudgetStatusDto | null> {
    try {
      const response = await api.get(`/budgets/status/${categoryId}`)
      return response.data.budget
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null
      }
      throw error
    }
  },

  // Delete a budget
  async delete(id: string): Promise<void> {
    await api.delete(`/budgets/${id}`)
  }
}
