export interface CategoryTypeDto {
  id: string
  name: string
  description?: string
  color?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateCategoryTypeDto {
  name: string
  description?: string
  color?: string
  isActive: boolean
}

export interface UpdateCategoryTypeDto {
  id: string
  name: string
  description?: string
  color?: string
  isActive: boolean
}
