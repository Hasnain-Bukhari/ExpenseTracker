import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import type { Account, Transaction, Goal, Budget } from '@/types'
import { mockAccounts, mockTransactions, mockGoals, mockBudgets } from '@/utils/mockData'

export const useAppStore = defineStore('app', () => {
  // State
  const accounts = ref<Account[]>(mockAccounts)
  const transactions = ref<Transaction[]>(mockTransactions)
  const goals = ref<Goal[]>(mockGoals)
  const budgets = ref<Budget[]>(mockBudgets)
  const sideNavCollapsed = ref(false)

  // Getters
  const totalBalance = computed(() => {
    return accounts.value.reduce((sum, account) => sum + account.balance, 0)
  })

  const monthlySpend = computed(() => {
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

  const savings = computed(() => monthlyIncome.value - monthlySpend.value)

  const recentTransactions = computed(() => {
    return transactions.value
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
      .slice(0, 8)
  })

  // Actions
  const toggleSideNav = () => {
    sideNavCollapsed.value = !sideNavCollapsed.value
  }

  return {
    // State
    accounts,
    transactions,
    goals,
    budgets,
    sideNavCollapsed,
    // Getters
    totalBalance,
    monthlySpend,
    monthlyIncome,
    savings,
    recentTransactions,
    // Actions
    toggleSideNav,
  }
})
