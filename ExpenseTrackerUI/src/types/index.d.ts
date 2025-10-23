export interface Account {
  id: string
  name: string
  type: 'checking' | 'savings' | 'credit' | 'investment'
  balance: number
  currency: string
}

export interface Transaction {
  id: string
  accountId: string
  account: string
  category: string
  description: string
  amount: number
  type: 'income' | 'expense'
  date: string
  status: 'completed' | 'pending' | 'cancelled'
}

export interface Goal {
  id: string
  name: string
  targetAmount: number
  currentAmount: number
  deadline: string
  category: string
  priority: 'high' | 'medium' | 'low'
}

export interface Budget {
  id: string
  category: string
  allocated: number
  spent: number
  period: 'monthly' | 'weekly' | 'yearly'
  startDate: string
  endDate: string
}

export interface NavItem {
  title: string
  icon: string
  to: string
  active?: boolean
}

// Re-export AccountType types
export * from './accountType'

// Re-export Account types
export * from './account'

// Re-export Currency types
export * from './currency'
