import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import type { Account, Transaction, Goal, Budget } from '@/types'
import { mockAccounts, mockTransactions, mockGoals, mockBudgets } from '@/utils/mockData'
import { dashboardService } from '@/lib/dashboardService'

export const useAppStore = defineStore('app', () => {
  // State
  const accounts = ref<Account[]>(mockAccounts)
  const transactions = ref<Transaction[]>(mockTransactions)
  const goals = ref<Goal[]>(mockGoals)
  const budgets = ref<Budget[]>(mockBudgets)
  const sideNavCollapsed = ref(false)

  // Dashboard summary state (fetched from API)
  const dashboardSummary = ref<{
    totalBalance: number
    monthlySpend: number
    monthlyIncome: number
    netSavings: number
  } | null>(null)

  // Getters - use real data from API if available, otherwise fall back to computed from mock data
  const totalBalance = computed(() => {
    if (dashboardSummary.value !== null) {
      return dashboardSummary.value.totalBalance
    }
    // Fallback to computed from accounts
    return accounts.value.reduce((sum, account) => sum + account.balance, 0)
  })

  const monthlySpend = computed(() => {
    if (dashboardSummary.value !== null) {
      return dashboardSummary.value.monthlySpend
    }
    // Fallback to computed from transactions
    const currentMonth = new Date().getMonth()
    const currentYear = new Date().getFullYear()
    
    return transactions.value
      .filter(t => {
        const date = new Date(t.date)
        return date.getMonth() === currentMonth && 
               date.getFullYear() === currentYear && 
               t.type === 'expense'
      })
      .reduce((sum, t) => sum + Math.abs(t.amount), 0)
  })

  const monthlyIncome = computed(() => {
    if (dashboardSummary.value !== null) {
      return dashboardSummary.value.monthlyIncome
    }
    // Fallback to computed from transactions
    const currentMonth = new Date().getMonth()
    const currentYear = new Date().getFullYear()
    
    return transactions.value
      .filter(t => {
        const date = new Date(t.date)
        return date.getMonth() === currentMonth && 
               date.getFullYear() === currentYear && 
               t.type === 'income'
      })
      .reduce((sum, t) => sum + t.amount, 0)
  })

  const savings = computed(() => {
    if (dashboardSummary.value !== null) {
      return dashboardSummary.value.netSavings
    }
    // Fallback to computed
    return monthlyIncome.value - monthlySpend.value
  })

  const recentTransactions = computed(() => {
    return transactions.value
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
      .slice(0, 8)
  })

  // Actions
  const toggleSideNav = () => {
    sideNavCollapsed.value = !sideNavCollapsed.value
  }

  const fetchDashboardSummary = async () => {
    try {
      const summary = await dashboardService.getSummary()
      dashboardSummary.value = summary
    } catch (error) {
      console.error('Failed to fetch dashboard summary:', error)
      // Keep existing value or null
    }
  }

  return {
    // State
    accounts,
    transactions,
    goals,
    budgets,
    sideNavCollapsed,
    dashboardSummary,
    // Getters
    totalBalance,
    monthlySpend,
    monthlyIncome,
    savings,
    recentTransactions,
    // Actions
    toggleSideNav,
    fetchDashboardSummary,
  }
})
