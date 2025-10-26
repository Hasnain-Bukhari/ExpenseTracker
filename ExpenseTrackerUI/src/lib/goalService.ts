import { api } from '@/lib/api'
import type { 
  CreateGoalDto, 
  UpdateGoalDto, 
  GoalDto, 
  GoalProgressDto 
} from '@/types/goal'

export const goalService = {
  // Create a new goal
  async create(goal: CreateGoalDto): Promise<GoalDto> {
    const response = await api.post('/goals', goal)
    return response.data
  },

  // Update an existing goal
  async update(goal: UpdateGoalDto): Promise<GoalDto> {
    const response = await api.put(`/goals/${goal.id}`, goal)
    return response.data
  },

  // Delete a goal
  async delete(id: string): Promise<void> {
    await api.delete(`/goals/${id}`)
  },

  // Get a specific goal by ID
  async getById(id: string): Promise<GoalDto> {
    const response = await api.get(`/goals/${id}`)
    return response.data
  },

  // Get all goals for the current user
  async getAll(): Promise<GoalDto[]> {
    const response = await api.get('/goals')
    return response.data
  },

  // Get active goals for the current user
  async getActive(): Promise<GoalDto[]> {
    const response = await api.get('/goals/active')
    return response.data
  },

  // Get completed goals for the current user
  async getCompleted(): Promise<GoalDto[]> {
    const response = await api.get('/goals/completed')
    return response.data
  },

  // Get goal progress for the current user
  async getProgress(): Promise<GoalProgressDto[]> {
    const response = await api.get('/goals/progress')
    return response.data
  },

  // Check if a goal exists
  async exists(id: string): Promise<boolean> {
    const response = await api.get(`/goals/${id}/exists`)
    return response.data
  }
}
