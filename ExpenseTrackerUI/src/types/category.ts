// Category API types

export enum CategoryType {
  Income = 'Income',
  Expense = 'Expense',
  TargetedSavingsGoal = 'TargetedSavingsGoal'
}

export interface CreateCategoryDto {
  name: string
  categoryType: CategoryType
  description?: string | null
}

export interface UpdateCategoryDto {
  id: string
  name: string
  categoryType: CategoryType
  description?: string | null
}

export interface CreateSubCategoryDto {
  name: string
  description?: string | null
}

export interface UpdateSubCategoryDto {
  name: string
  description?: string | null
}

export interface SubCategoryDto {
  id: string
  categoryId: string
  name: string
  description?: string | null
  createdAt: string
  updatedAt: string
}

export interface CategoryDto {
  id: string
  userId: string
  name: string
  categoryType: CategoryType
  description?: string | null
  createdAt: string
  updatedAt: string
  subCategories?: SubCategoryDto[] | null
}

export interface CategoriesListResponse {
  ok: boolean
  categories: CategoryDto[]
}
