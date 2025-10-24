import { useToast } from 'vue-toastification'

export interface ToastMessage {
  message: string
  title?: string
  duration?: number
}

class ToastService {
  private toast = useToast()

  success(message: string, title?: string, duration?: number) {
    this.toast.success(message, {
      title: title || 'Success',
      timeout: duration || 4000,
    })
  }

  error(message: string, title?: string, duration?: number) {
    this.toast.error(message, {
      title: title || 'Error',
      timeout: duration || 6000,
    })
  }

  warning(message: string, title?: string, duration?: number) {
    this.toast.warning(message, {
      title: title || 'Warning',
      timeout: duration || 5000,
    })
  }

  info(message: string, title?: string, duration?: number) {
    this.toast.info(message, {
      title: title || 'Info',
      timeout: duration || 4000,
    })
  }

  // Specific success messages for common operations
  created(resource: string) {
    this.success(`${resource} created successfully!`)
  }

  updated(resource: string) {
    this.success(`${resource} updated successfully!`)
  }

  deleted(resource: string) {
    this.success(`${resource} deleted successfully!`)
  }

  saved(resource: string) {
    this.success(`${resource} saved successfully!`)
  }

  // Specific error messages for common operations
  createFailed(resource: string, error?: string) {
    this.error(`Failed to create ${resource}. ${error || 'Please try again.'}`)
  }

  updateFailed(resource: string, error?: string) {
    this.error(`Failed to update ${resource}. ${error || 'Please try again.'}`)
  }

  deleteFailed(resource: string, error?: string) {
    this.error(`Failed to delete ${resource}. ${error || 'Please try again.'}`)
  }

  loadFailed(resource: string, error?: string) {
    this.error(`Failed to load ${resource}. ${error || 'Please try again.'}`)
  }

  // Authentication related messages
  loginSuccess() {
    this.success('Welcome back!', 'Login Successful')
  }

  loginFailed(error?: string) {
    this.error(`Login failed. ${error || 'Please check your credentials.'}`, 'Authentication Error')
  }

  logoutSuccess() {
    this.success('You have been logged out successfully.', 'Logout')
  }

  registerSuccess() {
    this.success('Account created successfully! Please log in.', 'Registration Successful')
  }

  registerFailed(error?: string) {
    this.error(`Registration failed. ${error || 'Please try again.'}`, 'Registration Error')
  }

  // Profile related messages
  profileUpdated() {
    this.success('Profile updated successfully!')
  }

  profileUpdateFailed(error?: string) {
    this.error(`Failed to update profile. ${error || 'Please try again.'}`)
  }

  passwordChanged() {
    this.success('Password changed successfully!')
  }

  passwordChangeFailed(error?: string) {
    this.error(`Failed to change password. ${error || 'Please try again.'}`)
  }

  // Transaction related messages
  transactionAdded() {
    this.success('Transaction added successfully!')
  }

  transactionAddFailed(error?: string) {
    this.error(`Failed to add transaction. ${error || 'Please try again.'}`)
  }

  transactionUpdated() {
    this.success('Transaction updated successfully!')
  }

  transactionUpdateFailed(error?: string) {
    this.error(`Failed to update transaction. ${error || 'Please try again.'}`)
  }

  transactionDeleted() {
    this.success('Transaction deleted successfully!')
  }

  transactionDeleteFailed(error?: string) {
    this.error(`Failed to delete transaction. ${error || 'Please try again.'}`)
  }

  // Currency related messages
  currencyCreated() {
    this.success('Currency created successfully!')
  }

  currencyCreateFailed(error?: string) {
    this.error(`Failed to create currency. ${error || 'Please try again.'}`)
  }

  currencyUpdated() {
    this.success('Currency updated successfully!')
  }

  currencyUpdateFailed(error?: string) {
    this.error(`Failed to update currency. ${error || 'Please try again.'}`)
  }

  currencyDeleted() {
    this.success('Currency deleted successfully!')
  }

  currencyDeleteFailed(error?: string) {
    this.error(`Failed to delete currency. ${error || 'Please try again.'}`)
  }

  // Account related messages
  accountCreated() {
    this.success('Account created successfully!')
  }

  accountCreateFailed(error?: string) {
    this.error(`Failed to create account. ${error || 'Please try again.'}`)
  }

  accountUpdated() {
    this.success('Account updated successfully!')
  }

  accountUpdateFailed(error?: string) {
    this.error(`Failed to update account. ${error || 'Please try again.'}`)
  }

  accountDeleted() {
    this.success('Account deleted successfully!')
  }

  accountDeleteFailed(error?: string) {
    this.error(`Failed to delete account. ${error || 'Please try again.'}`)
  }

  // Category related messages
  categoryCreated() {
    this.success('Category created successfully!')
  }

  categoryCreateFailed(error?: string) {
    this.error(`Failed to create category. ${error || 'Please try again.'}`)
  }

  categoryUpdated() {
    this.success('Category updated successfully!')
  }

  categoryUpdateFailed(error?: string) {
    this.error(`Failed to update category. ${error || 'Please try again.'}`)
  }

  categoryDeleted() {
    this.success('Category deleted successfully!')
  }

  categoryDeleteFailed(error?: string) {
    this.error(`Failed to delete category. ${error || 'Please try again.'}`)
  }

  // Account Type related messages
  accountTypeCreated() {
    this.success('Account type created successfully!')
  }

  accountTypeCreateFailed(error?: string) {
    this.error(`Failed to create account type. ${error || 'Please try again.'}`)
  }

  accountTypeUpdated() {
    this.success('Account type updated successfully!')
  }

  accountTypeUpdateFailed(error?: string) {
    this.error(`Failed to update account type. ${error || 'Please try again.'}`)
  }

  accountTypeDeleted() {
    this.success('Account type deleted successfully!')
  }

  accountTypeDeleteFailed(error?: string) {
    this.error(`Failed to delete account type. ${error || 'Please try again.'}`)
  }

  // Generic API error handler
  handleApiError(error: any, operation: string) {
    let message = 'An unexpected error occurred. Please try again.'
    
    if (error?.response?.data?.error) {
      message = error.response.data.error
    } else if (error?.response?.data?.message) {
      message = error.response.data.message
    } else if (error?.message) {
      message = error.message
    }

    this.error(`${operation} failed: ${message}`)
  }

  // Network error handler
  handleNetworkError(operation: string) {
    this.error(`Network error: Unable to ${operation.toLowerCase()}. Please check your connection.`)
  }
}

// Export singleton instance
export const toastService = new ToastService()
export default toastService
