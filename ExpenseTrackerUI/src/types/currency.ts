export interface CurrencyDto {
  id: string
  code: string
  name: string
  symbol: string
  createdAt: string
  updatedAt: string
}

export interface CreateCurrencyDto {
  code: string
  name: string
  symbol: string
}

export interface UpdateCurrencyDto {
  id: string
  code: string
  name: string
  symbol: string
}
