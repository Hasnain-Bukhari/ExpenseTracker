export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface BaseEntity {
  id: string
  createdAt: string
  updatedAt: string
}
