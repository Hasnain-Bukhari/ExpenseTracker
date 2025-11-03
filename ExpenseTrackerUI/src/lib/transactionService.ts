import { api } from '@/lib/api'
import type { TransactionDto } from '@/types/transaction'

export interface SpendingTrendData {
  labels: string[]
  expenses: number[]
  income: number[]
  goals?: number[]
}

export interface MonthlySpending {
  month: string
  expenses: number
  income: number
}

export const transactionService = {
  // Get spending trends for different periods
  async getSpendingTrends(period: '1m' | '3m' | '6m' | '1y' | '5y' | 'all'): Promise<SpendingTrendData> {
    const now = new Date()
    let startDate: Date
    let labels: string[]
    let groupBy: 'month' | 'quarter' = 'month'

    switch (period) {
      case '1m':
        startDate = new Date(now.getFullYear(), now.getMonth() - 1, 1)
        labels = this.generateMonthLabels(1)
        break
      case '3m':
        startDate = new Date(now.getFullYear(), now.getMonth() - 3, 1)
        labels = this.generateMonthLabels(3)
        break
      case '6m':
        startDate = new Date(now.getFullYear(), now.getMonth() - 6, 1)
        labels = this.generateMonthLabels(6)
        break
      case '1y':
        startDate = new Date(now.getFullYear() - 1, now.getMonth(), 1)
        labels = this.generateMonthLabels(12)
        break
      case '5y':
        startDate = new Date(now.getFullYear() - 5, now.getMonth(), 1)
        labels = this.generateYearLabels(5)
        groupBy = 'quarter'
        break
      case 'all':
        startDate = new Date(2023, 0, 1) // Start from 2023
        labels = this.generateQuarterLabels()
        groupBy = 'quarter'
        break
    }

    const endDate = new Date(now.getFullYear(), now.getMonth() + 1, 0) // End of current month

    try {
      // Fetch all transactions for the period
      const response = await api.get('/transactions', {
        params: {
          startDate: startDate.toISOString().split('T')[0],
          endDate: endDate.toISOString().split('T')[0],
          page: 1,
          pageSize: 1000 // Get all transactions for the period
        }
      })

      const transactions: TransactionDto[] = response.data.items || []
      
      // Group transactions by month/quarter
      const groupedData = this.groupTransactionsByPeriod(transactions, groupBy, labels.length)
      
      return {
        labels,
        expenses: groupedData.expenses,
        income: groupedData.income,
        goals: groupedData.goals
      }
    } catch (error) {
      console.error('Failed to fetch spending trends:', error)
      // Return empty data on error
      return {
        labels,
        expenses: new Array(labels.length).fill(0),
        income: new Array(labels.length).fill(0)
      }
    }
  },

  // Generate month labels for the last N months
  generateMonthLabels(months: number): string[] {
    const labels: string[] = []
    const now = new Date()
    
    for (let i = months - 1; i >= 0; i--) {
      const date = new Date(now.getFullYear(), now.getMonth() - i, 1)
      labels.push(date.toLocaleDateString('en-US', { month: 'short' }))
    }
    
    return labels
  },

  // Generate quarter labels
  generateQuarterLabels(): string[] {
    const labels: string[] = []
    const now = new Date()
    const currentYear = now.getFullYear()
    
    // Generate quarters from 2023 Q1 to current quarter
    for (let year = 2023; year <= currentYear; year++) {
      const maxQuarter = year === currentYear ? Math.ceil((now.getMonth() + 1) / 3) : 4
      for (let quarter = 1; quarter <= maxQuarter; quarter++) {
        labels.push(`${year} Q${quarter}`)
      }
    }
    
    return labels
  },

  // Generate year labels for the last N years
  generateYearLabels(years: number): string[] {
    const labels: string[] = []
    const now = new Date()
    
    for (let i = years - 1; i >= 0; i--) {
      const year = now.getFullYear() - i
      labels.push(year.toString())
    }
    
    return labels
  },

  // Group transactions by period and calculate totals
  groupTransactionsByPeriod(transactions: TransactionDto[], groupBy: 'month' | 'quarter', periodCount: number): { expenses: number[], income: number[], goals: number[] } {
    const expenses = new Array(periodCount).fill(0)
    const income = new Array(periodCount).fill(0)
    const goals = new Array(periodCount).fill(0)
    
    transactions.forEach(transaction => {
      const transactionDate = new Date(transaction.transactionDate)
      let periodIndex: number
      
      if (groupBy === 'month') {
        // Calculate months back from current month
        const now = new Date()
        const monthsDiff = (now.getFullYear() - transactionDate.getFullYear()) * 12 + (now.getMonth() - transactionDate.getMonth())
        periodIndex = periodCount - 1 - monthsDiff
      } else {
        // Calculate quarters back from current quarter
        const now = new Date()
        const currentQuarter = Math.ceil((now.getMonth() + 1) / 3)
        const transactionQuarter = Math.ceil((transactionDate.getMonth() + 1) / 3)
        const quartersDiff = (now.getFullYear() - transactionDate.getFullYear()) * 4 + (currentQuarter - transactionQuarter)
        periodIndex = periodCount - 1 - quartersDiff
      }
      
      if (periodIndex >= 0 && periodIndex < periodCount) {
        // Categorize based on categoryType field
        if (transaction.category?.categoryType === 'Income') {
          income[periodIndex] += Math.abs(transaction.amount)
        } else if (transaction.category?.categoryType === 'Expense') {
          expenses[periodIndex] += Math.abs(transaction.amount)
        } else if (transaction.category?.categoryType === 'TargetedSavingsGoal') {
          goals[periodIndex] += Math.abs(transaction.amount)
        }
      }
    })
    
    return { expenses, income, goals }
  },

  // Get current month spending
  async getCurrentMonthSpending(): Promise<number> {
    const now = new Date()
    const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1)
    const endOfMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0)
    
    try {
      const response = await api.get('/transactions', {
        params: {
          startDate: startOfMonth.toISOString().split('T')[0],
          endDate: endOfMonth.toISOString().split('T')[0],
          page: 1,
          pageSize: 1000
        }
      })
      
      const transactions: TransactionDto[] = response.data.items || []
      return transactions
        .filter(t => t.category?.categoryType === 'Expense')
        .reduce((sum, t) => sum + Math.abs(t.amount), 0)
    } catch (error) {
      console.error('Failed to fetch current month spending:', error)
      return 0
    }
  },

  // Get average monthly spending
  async getAverageMonthlySpending(): Promise<number> {
    const now = new Date()
    const startOfYear = new Date(now.getFullYear(), 0, 1)
    const endOfYear = new Date(now.getFullYear(), 11, 31)
    
    try {
      const response = await api.get('/transactions', {
        params: {
          startDate: startOfYear.toISOString().split('T')[0],
          endDate: endOfYear.toISOString().split('T')[0],
          page: 1,
          pageSize: 1000
        }
      })
      
      const transactions: TransactionDto[] = response.data.items || []
      const monthlyTotals = new Map<string, number>()
      
      transactions.forEach(transaction => {
        if (transaction.category?.categoryType === 'Expense') {
          const monthKey = new Date(transaction.transactionDate).toISOString().substring(0, 7) // YYYY-MM
          monthlyTotals.set(monthKey, (monthlyTotals.get(monthKey) || 0) + Math.abs(transaction.amount))
        }
      })
      
      const totalSpending = Array.from(monthlyTotals.values()).reduce((sum, amount) => sum + amount, 0)
      const monthCount = monthlyTotals.size || 1
      
      return totalSpending / monthCount
    } catch (error) {
      console.error('Failed to fetch average monthly spending:', error)
      return 0
    }
  }
}
