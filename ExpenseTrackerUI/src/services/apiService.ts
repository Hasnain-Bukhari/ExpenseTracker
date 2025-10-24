import { toastService } from '@/services/toastService'
import { 
  accountTypeApi, 
  currencyApi, 
  categoryTypeApi, 
  categoryApi, 
  accountApi, 
  transactionApi, 
  profileApi 
} from '@/lib/api'

// AccountType API with toast notifications
export const accountTypeService = {
  async list() {
    try {
      const data = await accountTypeApi.list()
      return data
    } catch (error) {
      toastService.loadFailed('account types')
      throw error
    }
  },

  async get(id: string) {
    try {
      const data = await accountTypeApi.get(id)
      return data
    } catch (error) {
      toastService.loadFailed('account type')
      throw error
    }
  },

  async create(data: { name: string; isCard: boolean }) {
    try {
      const result = await accountTypeApi.create(data)
      toastService.accountTypeCreated()
      return result
    } catch (error) {
      toastService.accountTypeCreateFailed()
      throw error
    }
  },

  async update(id: string, data: { name: string; isCard: boolean }) {
    try {
      const result = await accountTypeApi.update(id, data)
      toastService.accountTypeUpdated()
      return result
    } catch (error) {
      toastService.accountTypeUpdateFailed()
      throw error
    }
  },

  async delete(id: string) {
    try {
      const result = await accountTypeApi.delete(id)
      toastService.accountTypeDeleted()
      return result
    } catch (error) {
      toastService.accountTypeDeleteFailed()
      throw error
    }
  }
}

// Currency API with toast notifications
export const currencyService = {
  async list() {
    try {
      const data = await currencyApi.list()
      return data
    } catch (error) {
      toastService.loadFailed('currencies')
      throw error
    }
  },

  async get(id: string) {
    try {
      const data = await currencyApi.get(id)
      return data
    } catch (error) {
      toastService.loadFailed('currency')
      throw error
    }
  },

  async create(data: { code: string; name: string; symbol: string }) {
    try {
      const result = await currencyApi.create(data)
      toastService.currencyCreated()
      return result
    } catch (error) {
      toastService.currencyCreateFailed()
      throw error
    }
  },

  async update(id: string, data: { id: string; code: string; name: string; symbol: string }) {
    try {
      const result = await currencyApi.update(id, data)
      toastService.currencyUpdated()
      return result
    } catch (error) {
      toastService.currencyUpdateFailed()
      throw error
    }
  },

  async delete(id: string) {
    try {
      const result = await currencyApi.delete(id)
      toastService.currencyDeleted()
      return result
    } catch (error) {
      toastService.currencyDeleteFailed()
      throw error
    }
  }
}

// CategoryType API with toast notifications
export const categoryTypeService = {
  async list() {
    try {
      const data = await categoryTypeApi.list()
      return data
    } catch (error) {
      toastService.loadFailed('category types')
      throw error
    }
  },

  async get(id: string) {
    try {
      const data = await categoryTypeApi.get(id)
      return data
    } catch (error) {
      toastService.loadFailed('category type')
      throw error
    }
  },

  async create(data: { name: string; description?: string; color?: string; isActive: boolean }) {
    try {
      const result = await categoryTypeApi.create(data)
      toastService.created('Category type')
      return result
    } catch (error) {
      toastService.createFailed('category type')
      throw error
    }
  },

  async update(id: string, data: { id: string; name: string; description?: string; color?: string; isActive: boolean }) {
    try {
      const result = await categoryTypeApi.update(id, data)
      toastService.updated('Category type')
      return result
    } catch (error) {
      toastService.updateFailed('category type')
      throw error
    }
  },

  async delete(id: string) {
    try {
      const result = await categoryTypeApi.delete(id)
      toastService.deleted('Category type')
      return result
    } catch (error) {
      toastService.deleteFailed('category type')
      throw error
    }
  }
}

// Category API with toast notifications
export const categoryService = {
  async list() {
    try {
      const data = await categoryApi.list()
      return data
    } catch (error) {
      toastService.loadFailed('categories')
      throw error
    }
  },

  async get(id: string) {
    try {
      const data = await categoryApi.get(id)
      return data
    } catch (error) {
      toastService.loadFailed('category')
      throw error
    }
  },

  async create(data: { name: string; categoryTypeId: string; description?: string | null }) {
    try {
      const result = await categoryApi.create(data)
      toastService.categoryCreated()
      return result
    } catch (error) {
      toastService.categoryCreateFailed()
      throw error
    }
  },

  async update(id: string, data: { id: string; name: string; categoryTypeId: string; description?: string | null }) {
    try {
      const result = await categoryApi.update(id, data)
      toastService.categoryUpdated()
      return result
    } catch (error) {
      toastService.categoryUpdateFailed()
      throw error
    }
  },

  async delete(id: string) {
    try {
      const result = await categoryApi.delete(id)
      toastService.categoryDeleted()
      return result
    } catch (error) {
      toastService.categoryDeleteFailed()
      throw error
    }
  },

  // Subcategory functions
  async createSubcategory(categoryId: string, data: { name: string; description?: string | null }) {
    try {
      const result = await categoryApi.createSubcategory(categoryId, data)
      toastService.created('Subcategory')
      return result
    } catch (error) {
      toastService.createFailed('subcategory')
      throw error
    }
  },

  async updateSubcategory(categoryId: string, subcategoryId: string, data: { name: string; description?: string | null }) {
    try {
      const result = await categoryApi.updateSubcategory(categoryId, subcategoryId, data)
      toastService.updated('Subcategory')
      return result
    } catch (error) {
      toastService.updateFailed('subcategory')
      throw error
    }
  },

  async deleteSubcategory(categoryId: string, subcategoryId: string) {
    try {
      const result = await categoryApi.deleteSubcategory(categoryId, subcategoryId)
      toastService.deleted('Subcategory')
      return result
    } catch (error) {
      toastService.deleteFailed('subcategory')
      throw error
    }
  }
}

// Account API with toast notifications
export const accountService = {
  async list() {
    try {
      const data = await accountApi.list()
      return data
    } catch (error) {
      toastService.loadFailed('accounts')
      throw error
    }
  },

  async get(id: string) {
    try {
      const data = await accountApi.get(id)
      return data
    } catch (error) {
      toastService.loadFailed('account')
      throw error
    }
  },

  async create(data: { 
    name: string
    accountTypeId: string
    currencyId: string
    isSavings: boolean
    openingBalance: number
    includeInNetworth: boolean
  }) {
    try {
      const result = await accountApi.create(data)
      toastService.accountCreated()
      return result
    } catch (error) {
      toastService.accountCreateFailed()
      throw error
    }
  },

  async update(id: string, data: { 
    id: string
    name: string
    accountTypeId: string
    currencyId: string
    isSavings: boolean
    openingBalance: number
    includeInNetworth: boolean
  }) {
    try {
      const result = await accountApi.update(id, data)
      toastService.accountUpdated()
      return result
    } catch (error) {
      toastService.accountUpdateFailed()
      throw error
    }
  },

  async delete(id: string) {
    try {
      const result = await accountApi.delete(id)
      toastService.accountDeleted()
      return result
    } catch (error) {
      toastService.accountDeleteFailed()
      throw error
    }
  }
}

// Transaction API with toast notifications
export const transactionService = {
  async list(filters?: {
    accountId?: string
    categoryId?: string
    startDate?: string
    endDate?: string
    page?: number
    pageSize?: number
  }) {
    try {
      const data = await transactionApi.list(filters)
      return data
    } catch (error) {
      toastService.loadFailed('transactions')
      throw error
    }
  },

  async get(id: string) {
    try {
      const data = await transactionApi.get(id)
      return data
    } catch (error) {
      toastService.loadFailed('transaction')
      throw error
    }
  },

  async create(data: {
    accountId: string
    categoryId: string
    subCategoryId: string
    description?: string | null
    amount: number
    transactionDate: string
  }) {
    try {
      const result = await transactionApi.create(data)
      toastService.transactionAdded()
      return result
    } catch (error) {
      toastService.transactionAddFailed()
      throw error
    }
  },

  async update(id: string, data: {
    id: string
    accountId: string
    categoryId: string
    subCategoryId: string
    description?: string | null
    amount: number
    transactionDate: string
  }) {
    try {
      const result = await transactionApi.update(id, data)
      toastService.transactionUpdated()
      return result
    } catch (error) {
      toastService.transactionUpdateFailed()
      throw error
    }
  },

  async delete(id: string) {
    try {
      const result = await transactionApi.delete(id)
      toastService.transactionDeleted()
      return result
    } catch (error) {
      toastService.transactionDeleteFailed()
      throw error
    }
  }
}

// Profile API with toast notifications
export const profileService = {
  async get() {
    try {
      const data = await profileApi.get()
      return data
    } catch (error) {
      toastService.loadFailed('profile')
      throw error
    }
  },

  async update(data: {
    fullName?: string | null
    phone?: string | null
    profileImage?: string | null
    defaultCurrencyId?: string | null
    defaultAccountId?: string | null
    locale?: string | null
    timezone?: string | null
  }) {
    try {
      const result = await profileApi.update(data)
      toastService.profileUpdated()
      return result
    } catch (error) {
      toastService.profileUpdateFailed()
      throw error
    }
  },

  async changePassword(data: {
    currentPassword: string
    newPassword: string
    confirmPassword: string
  }) {
    try {
      const result = await profileApi.changePassword(data)
      toastService.passwordChanged()
      return result
    } catch (error) {
      toastService.passwordChangeFailed()
      throw error
    }
  },

  async updateImage(imageUrl: string) {
    try {
      const result = await profileApi.updateImage(imageUrl)
      toastService.success('Profile image updated successfully!')
      return result
    } catch (error) {
      toastService.error('Failed to update profile image. Please try again.')
      throw error
    }
  },

  async getAccountsByCurrency(currencyId: string) {
    try {
      const data = await profileApi.getAccountsByCurrency(currencyId)
      return data
    } catch (error) {
      toastService.loadFailed('accounts for selected currency')
      throw error
    }
  }
}
