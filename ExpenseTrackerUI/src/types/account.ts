export interface AccountDto {
  id: string
  userId: string
  name: string
  accountTypeId: string
  currencyId: string
  isSavings: boolean
  openingBalance: number
  includeInNetworth: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateAccountDto {
  name: string
  accountTypeId: string
  currencyId: string
  isSavings: boolean
  openingBalance: number
  includeInNetworth: boolean
}

export interface UpdateAccountDto {
  id: string
  name: string
  accountTypeId: string
  currencyId: string
  isSavings: boolean
  openingBalance: number
  includeInNetworth: boolean
}
