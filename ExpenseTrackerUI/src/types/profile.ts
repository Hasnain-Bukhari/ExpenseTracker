export interface ProfileDto {
  id: string
  email: string
  fullName?: string | null
  preferredName?: string | null
  phone?: string | null
  profileImage?: string | null
  defaultCurrencyId?: string | null
  defaultCurrency?: {
    id: string
    userId?: string | null
    code: string
    symbol?: string | null
    name?: string | null
    createdAt: string
    updatedAt: string
  } | null
  defaultAccountId?: string | null
  defaultAccount?: {
    id: string
    userId: string
    name: string
    accountTypeId: string
    currencyId: string
    isSavings: boolean
    createdAt: string
    updatedAt: string
  } | null
  locale?: string | null
  timezone?: string | null
  isEmailVerified: boolean
  provider: 'Local' | 'Google' | 'Facebook' | 'Mixed'
  providerId?: string | null
  lastLoginAt?: string | null
  createdAt: string
  updatedAt: string
}

export interface UpdateProfileDto {
  fullName?: string | null
  preferredName?: string | null
  phone?: string | null
  profileImage?: string | null
  defaultCurrencyId?: string | null
  defaultAccountId?: string | null
  locale?: string | null
  timezone?: string | null
}

export interface ChangePasswordDto {
  currentPassword: string
  newPassword: string
  confirmPassword: string
}

export interface ProfileImageDto {
  imageUrl: string
}
