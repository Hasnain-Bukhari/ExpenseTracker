import type { AccountDto } from './account'
import type { CategoryDto, SubCategoryDto } from './category'
import { CategoryType } from './category'

export interface CreateTransactionDto {
  accountId: string
  categoryId: string
  subCategoryId: string
  description?: string | null
  amount: number
  transactionDate: string
}

export interface UpdateTransactionDto {
  id: string
  accountId: string
  categoryId: string
  subCategoryId: string
  description?: string | null
  amount: number
  transactionDate: string
}

export interface TransactionDto {
  id: string
  userId: string
  accountId: string
  categoryId: string
  subCategoryId?: string | null
  description: string
  amount: number
  transactionDate: string
  createdAt: string
  updatedAt: string
  account: AccountDto
  category: CategoryDto
  subCategory?: SubCategoryDto | null
}

export interface TransactionListDto {
  id: string
  description: string
  amount: number
  transactionDate: string
  accountName: string
  categoryName: string
  subCategoryName?: string | null
  categoryType: CategoryType
}
