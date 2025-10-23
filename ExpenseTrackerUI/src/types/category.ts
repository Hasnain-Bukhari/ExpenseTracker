// Category API types
import type { CategoryTypeDto } from './categoryType'

export interface CreateCategoryDto {
  name: string
  categoryTypeId: string
  description?: string | null
}

export interface UpdateCategoryDto {
  id: string
  name: string
  categoryTypeId: string
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
  categoryTypeId: string
  description?: string | null
  createdAt: string
  updatedAt: string
  categoryType: CategoryTypeDto
  subCategories?: SubCategoryDto[] | null
}

export interface CategoriesListResponse {
  ok: boolean
  categories: CategoryDto[]
}
