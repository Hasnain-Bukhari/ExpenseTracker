export interface AccountTypeDto {
  id: string
  name: string
  isCard: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateAccountTypeDto {
  name: string
  isCard: boolean
}

export interface UpdateAccountTypeDto {
  name: string
  isCard: boolean
}
