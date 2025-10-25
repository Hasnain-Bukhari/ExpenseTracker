import { api } from '@/lib/api'
import type { 
  CreateCategoryDto, 
  UpdateCategoryDto, 
  CategoryDto,
  CreateSubCategoryDto,
  UpdateSubCategoryDto,
  SubCategoryDto
} from '@/types/category'

export const categoryService = {
  // Get all categories
  async list(): Promise<CategoryDto[]> {
    const response = await api.get<CategoryDto[]>('/categories')
    return response.data
  },

  // Get only expense categories
  async getExpenseCategories(): Promise<CategoryDto[]> {
    const response = await api.get<CategoryDto[]>('/categories?categoryType=Expense')
    return response.data
  },

  // Create a new category
  async create(data: CreateCategoryDto): Promise<CategoryDto> {
    const response = await api.post('/categories', data)
    return response.data.category
  },

  // Update an existing category
  async update(id: string, data: UpdateCategoryDto): Promise<CategoryDto> {
    const response = await api.put(`/categories/${id}`, data)
    return response.data.category
  },

  // Delete a category
  async delete(id: string): Promise<void> {
    await api.delete(`/categories/${id}`)
  },

  // Subcategory operations
  async createSubCategory(categoryId: string, data: CreateSubCategoryDto): Promise<SubCategoryDto> {
    const response = await api.post(`/categories/${categoryId}/subcategories`, data)
    return response.data.subCategory
  },

  async updateSubCategory(categoryId: string, subCategoryId: string, data: UpdateSubCategoryDto): Promise<SubCategoryDto> {
    const response = await api.put(`/categories/${categoryId}/subcategories/${subCategoryId}`, data)
    return response.data.subCategory
  },

  async deleteSubCategory(categoryId: string, subCategoryId: string): Promise<void> {
    await api.delete(`/categories/${categoryId}/subcategories/${subCategoryId}`)
  }
}
