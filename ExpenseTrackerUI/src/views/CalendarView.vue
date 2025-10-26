<template>
  <v-app>
    <AppHeader />
    <AppNav />
    
    <v-main class="main-content">
      <div class="container-custom pa-3 pa-sm-6">
        <!-- Header Section -->
        <div class="calendar-header mb-6">
          <div class="d-flex align-center justify-space-between mb-4">
            <div>
              <h1 class="text-h4 text-sm-h3 font-weight-bold mb-2">Transaction Calendar</h1>
              <p class="text-subtitle-1 text-secondary">View and manage your transactions by date</p>
            </div>
            
            <!-- Month Navigation -->
            <div class="d-flex align-center gap-3">
              <v-btn
                icon
                size="large"
                variant="outlined"
                @click="previousMonth"
                :disabled="isLoading"
              >
                <v-icon>mdi-chevron-left</v-icon>
              </v-btn>
              
              <div class="text-center">
                <h2 class="text-h5 font-weight-bold">{{ currentMonthYear }}</h2>
                <p class="text-caption text-secondary">{{ monthlyStats.transactionCount }} transactions</p>
              </div>
              
              <v-btn
                icon
                size="large"
                variant="outlined"
                @click="nextMonth"
                :disabled="isLoading"
              >
                <v-icon>mdi-chevron-right</v-icon>
              </v-btn>
            </div>
          </div>
          
          <!-- Quick Stats -->
          <v-row>
            <v-col cols="12" sm="3">
              <v-card class="stat-card" variant="outlined">
                <v-card-text class="text-center">
                  <v-icon icon="mdi-calendar-check" color="primary" size="32" class="mb-2" />
                  <p class="text-h6 font-weight-bold text-primary">{{ monthlyStats.transactionCount }}</p>
                  <p class="text-caption text-secondary">Transactions</p>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12" sm="3">
              <v-card class="stat-card" variant="outlined">
                <v-card-text class="text-center">
                  <v-icon icon="mdi-currency-usd" color="success" size="32" class="mb-2" />
                  <p class="text-h6 font-weight-bold text-success">{{ formatCurrency(monthlyStats.totalAmount) }}</p>
                  <p class="text-caption text-secondary">Total Amount</p>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12" sm="3">
              <v-card class="stat-card" variant="outlined">
                <v-card-text class="text-center">
                  <v-icon icon="mdi-trending-up" color="info" size="32" class="mb-2" />
                  <p class="text-h6 font-weight-bold text-info">{{ formatCurrency(monthlyStats.incomeAmount) }}</p>
                  <p class="text-caption text-secondary">Income</p>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12" sm="3">
              <v-card class="stat-card" variant="outlined">
                <v-card-text class="text-center">
                  <v-icon icon="mdi-trending-down" color="error" size="32" class="mb-2" />
                  <p class="text-h6 font-weight-bold text-error">{{ formatCurrency(monthlyStats.expenseAmount) }}</p>
                  <p class="text-caption text-secondary">Expenses</p>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </div>

        <!-- Calendar Grid -->
        <v-card class="calendar-card" variant="outlined">
          <v-card-text class="pa-0">
            <!-- Weekday Headers -->
            <div class="calendar-weekdays">
              <div v-for="day in weekdays" :key="day" class="weekday-header">
                {{ day }}
              </div>
            </div>
            
            <!-- Calendar Days -->
            <div class="calendar-grid">
              <div
                v-for="(day, index) in calendarDays"
                :key="index"
                :class="[
                  'calendar-day',
                  {
                    'calendar-day--other-month': !day.isCurrentMonth,
                    'calendar-day--today': day.isToday,
                    'calendar-day--selected': selectedDate && day.date.toDateString() === selectedDate.toDateString(),
                    'calendar-day--has-transactions': day.transactionCount > 0
                  }
                ]"
                @click="selectDate(day)"
              >
                <div class="day-header">
                  <span class="day-number">{{ day.dayNumber }}</span>
                  <span v-if="day.transactionCount > 0" class="transaction-count">
                    {{ day.transactionCount }}
                  </span>
                </div>
                
                <!-- Transaction Indicators -->
                <div v-if="day.transactionCount > 0" class="transaction-indicators">
                  <div
                    v-for="(transaction, txIndex) in day.transactions.slice(0, 4)"
                    :key="txIndex"
                    class="transaction-indicator"
                    :style="{ backgroundColor: getCategoryTypeColor(transaction.category?.categoryType) }"
                    :title="`${transaction.description} - ${formatCurrency(transaction.amount)}`"
                  ></div>
                  <div v-if="day.transactionCount > 4" class="more-indicator">
                    +{{ day.transactionCount - 4 }}
                  </div>
                </div>
              </div>
            </div>
          </v-card-text>
        </v-card>

      </div>
    </v-main>

    <AppFooter />

    <!-- Transactions Dialog -->
    <v-dialog v-model="showTransactionsDialog" max-width="900px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-calendar-today" class="mr-2" />
          Transactions for {{ selectedDate ? formatDate(selectedDate) : '' }}
          <v-spacer></v-spacer>
          <v-btn
            icon
            variant="text"
            @click="showTransactionsDialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-card-title>
        
        <v-card-text>
          <!-- Loading State -->
          <div v-if="isLoading" class="text-center py-8">
            <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
            <p class="mt-4 text-body-1">Loading transactions...</p>
          </div>

          <!-- Transactions List -->
          <div v-else-if="selectedDateTransactions.length > 0">
            <div class="transaction-list">
              <v-card
                v-for="transaction in selectedDateTransactions"
                :key="transaction.id"
                class="transaction-card mb-4"
                variant="outlined"
                rounded="xl"
              >
                <v-card-text class="pa-4">
                  <div class="d-flex align-center">
                    <!-- Transaction Icon -->
                    <v-avatar
                      :color="getCategoryTypeColor(transaction.category?.categoryType)"
                      size="56"
                      class="mr-4"
                    >
                      <v-icon :color="getTransactionIconColor()" size="28">
                        {{ getTransactionIcon(transaction.category?.categoryType) }}
                      </v-icon>
                    </v-avatar>
                    
                    <!-- Transaction Details -->
                    <div class="flex-grow-1">
                      <div class="d-flex align-center justify-space-between mb-2">
                        <h3 class="text-h6 font-weight-medium">
                          {{ transaction.description || 'No description' }}
                        </h3>
                        <div class="text-right">
                          <p class="text-h5 font-weight-bold mb-1" :class="getAmountColor(transaction.category?.categoryType)">
                            {{ formatCurrency(transaction.amount) }}
                          </p>
                          <p class="text-caption text-secondary">
                            {{ new Date(transaction.transactionDate).toLocaleTimeString('en-US', { 
                              hour: '2-digit', 
                              minute: '2-digit' 
                            }) }}
                          </p>
                        </div>
                      </div>
                      
                      <!-- Category and Account Info -->
                      <div class="d-flex align-center gap-3">
                        <v-chip
                          :color="getCategoryTypeColor(transaction.category?.categoryType)"
                          size="small"
                          variant="tonal"
                          prepend-icon="mdi-tag"
                        >
                          {{ transaction.category?.name }}
                        </v-chip>
                        
                        <v-chip
                          v-if="transaction.subCategory?.name"
                          color="grey-lighten-1"
                          size="small"
                          variant="outlined"
                          prepend-icon="mdi-tag-outline"
                        >
                          {{ transaction.subCategory.name }}
                        </v-chip>
                        
                        <div class="d-flex align-center text-caption text-secondary">
                          <v-icon icon="mdi-bank" size="16" class="mr-1"></v-icon>
                          {{ transaction.account?.name }}
                        </div>
                      </div>
                    </div>
                  </div>
                </v-card-text>
              </v-card>
            </div>
          </div>

          <!-- Empty State -->
          <div v-else class="text-center py-8">
            <v-icon icon="mdi-calendar-blank" size="64" color="grey-lighten-1" class="mb-4" />
            <h3 class="text-h6 mb-2">No transactions on this date</h3>
            <p class="text-body-2 text-secondary">Select a different date or add a new transaction.</p>
            <v-btn
              color="primary"
              prepend-icon="mdi-plus"
              @click="showTransactionsDialog = false"
              rounded="xl"
              class="text-capitalize mt-4"
            >
              Add Transaction
            </v-btn>
          </div>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { formatCurrency } from '@/utils/formatters'
import { transactionService } from '@/services/apiService'
import type { TransactionDto } from '@/types/transaction'
import { CategoryType } from '@/types/category'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'

const route = useRoute()
const isLoading = ref(false)
const currentDate = ref(new Date())
const selectedDate = ref<Date | null>(null)
const transactions = ref<TransactionDto[]>([])
const showTransactionsDialog = ref(false)

// Calendar data
const weekdays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']

// Helper functions
const getCategoryTypeColor = (categoryType: CategoryType): string => {
  switch (categoryType) {
    case CategoryType.Income: return '#4caf50'
    case CategoryType.Expense: return '#f44336'
    case CategoryType.TargetedSavingsGoal: return '#2196f3'
    default: return '#9e9e9e'
  }
}

// Computed properties
const currentMonthYear = computed(() => {
  return currentDate.value.toLocaleDateString('en-US', { 
    month: 'long', 
    year: 'numeric' 
  })
})

const calendarDays = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  const today = new Date()
  
  // First day of the month
  const firstDay = new Date(year, month, 1)
  
  // Start from Sunday of the first week
  const startDate = new Date(firstDay)
  startDate.setDate(startDate.getDate() - firstDay.getDay())
  
  const days = []
  const current = new Date(startDate)
  
  // Generate 42 days (6 weeks)
  for (let i = 0; i < 42; i++) {
    const isCurrentMonth = current.getMonth() === month
    const isToday = current.toDateString() === today.toDateString()
    const dayTransactions = getTransactionsForDate(current)
    
    days.push({
      date: new Date(current),
      dayNumber: current.getDate(),
      isCurrentMonth,
      isToday,
      transactions: dayTransactions,
      transactionCount: dayTransactions.length
    })
    
    current.setDate(current.getDate() + 1)
  }
  
  return days
})

const monthlyStats = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  
  const monthTransactions = transactions.value.filter(tx => {
    const txDate = new Date(tx.transactionDate)
    return txDate.getFullYear() === year && txDate.getMonth() === month
  })
  
  const incomeTransactions = monthTransactions.filter(tx => {
    return tx.category?.categoryType === CategoryType.Income
  })
  
  const expenseTransactions = monthTransactions.filter(tx => {
    return tx.category?.categoryType === CategoryType.Expense
  })
  
  const goalTransactions = monthTransactions.filter(tx => {
    return tx.category?.categoryType === CategoryType.TargetedSavingsGoal
  })
  
  return {
    transactionCount: monthTransactions.length,
    totalAmount: monthTransactions.reduce((sum, tx) => sum + tx.amount, 0),
    incomeAmount: incomeTransactions.reduce((sum, tx) => sum + tx.amount, 0),
    expenseAmount: expenseTransactions.reduce((sum, tx) => sum + tx.amount, 0)
  }
})

const selectedDateTransactions = computed(() => {
  if (!selectedDate.value) return []
  return getTransactionsForDate(selectedDate.value)
})

// Methods
const getTransactionsForDate = (date: Date): TransactionDto[] => {
  const dateString = date.toISOString().split('T')[0]
  console.log(`CalendarView - Looking for date: ${dateString}`)
  console.log(`CalendarView - Available transaction dates:`, transactions.value.map(tx => tx.transactionDate).slice(0, 5))
  
  const dayTransactions = transactions.value.filter(tx => {
    // Try different date formats
    const txDate = tx.transactionDate
    const txDateOnly = txDate.split('T')[0] // Handle both "2025-10-24" and "2025-10-24T00:00:00Z"
    console.log(`CalendarView - Comparing ${dateString} with ${txDate} (${txDateOnly})`)
    return txDateOnly === dateString
  })
  
  console.log(`CalendarView - Date: ${dateString}, Found ${dayTransactions.length} transactions`)
  if (dayTransactions.length > 0) {
    console.log('Sample transaction:', dayTransactions[0])
  }
  return dayTransactions
}

const loadTransactions = async () => {
  isLoading.value = true
  try {
    // Calculate start and end dates for the current month
    const startOfMonth = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth(), 1)
    const endOfMonth = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 0)
    
    console.log('CalendarView - Loading transactions for month:', currentDate.value.getMonth() + 1, currentDate.value.getFullYear())
    console.log('CalendarView - Date range:', startOfMonth.toISOString().split('T')[0], 'to', endOfMonth.toISOString().split('T')[0])
    
    const response = await transactionService.list({ 
      page: 1, 
      pageSize: 1000,
      startDate: startOfMonth.toISOString().split('T')[0],
      endDate: endOfMonth.toISOString().split('T')[0]
    })
    console.log('Calendar View - Transaction response:', response)
    
    // Handle both direct array and paged result
    if (Array.isArray(response)) {
      transactions.value = response
    } else if (response && response.items) {
      transactions.value = response.items
    } else {
      transactions.value = []
    }
    
            console.log('Calendar View - Processed transactions:', transactions.value.length)
            console.log('Calendar View - Sample transaction:', transactions.value[0])
            console.log('Calendar View - Transaction dates:', transactions.value.map(tx => tx.transactionDate).slice(0, 5))
            console.log('Calendar View - First transaction date format:', transactions.value[0]?.transactionDate)
  } catch (error) {
    console.error('Failed to load transactions:', error)
    transactions.value = []
  } finally {
    isLoading.value = false
  }
}

const previousMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() - 1, 1)
  selectedDate.value = null
  loadTransactions()
}

const nextMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 1)
  selectedDate.value = null
  loadTransactions()
}

const selectDate = (day: any) => {
  selectedDate.value = day.date
  showTransactionsDialog.value = true
}

const formatDate = (date: Date): string => {
  return date.toLocaleDateString('en-US', { 
    weekday: 'long',
    year: 'numeric', 
    month: 'long', 
    day: 'numeric' 
  })
}

const getTransactionIcon = (categoryType: CategoryType | undefined): string => {
  if (!categoryType) return 'mdi-help-circle'
  switch (categoryType) {
    case CategoryType.Income: return 'mdi-trending-up'
    case CategoryType.Expense: return 'mdi-trending-down'
    case CategoryType.TargetedSavingsGoal: return 'mdi-target'
    default: return 'mdi-help-circle'
  }
}

const getTransactionIconColor = (): string => {
  return 'white'
}

const getAmountColor = (categoryType: CategoryType | undefined): string => {
  if (!categoryType) return 'text-secondary'
  switch (categoryType) {
    case CategoryType.Income: return 'text-success'
    case CategoryType.Expense: return 'text-error'
    case CategoryType.TargetedSavingsGoal: return 'text-info'
    default: return 'text-secondary'
  }
}

// Watch for month changes to reload data
watch(currentDate, () => {
  loadTransactions()
})

// Initialize selected date from route query
onMounted(() => {
  loadTransactions()
  
  // Check if there's a date in the query parameters
  if (route.query.date) {
    const queryDate = new Date(route.query.date as string)
    if (!isNaN(queryDate.getTime())) {
      selectedDate.value = queryDate
      // Update current date to show the month containing the selected date
      currentDate.value = new Date(queryDate.getFullYear(), queryDate.getMonth(), 1)
    }
  }
})
</script>

<style scoped>
.calendar-header {
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.05) 0%, transparent 100%);
  border-radius: 16px;
  padding: 24px;
  margin-bottom: 24px;
}

.stat-card {
  border-radius: 12px;
  transition: all 0.2s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(var(--v-theme-primary), 0.15);
}

.calendar-card {
  border-radius: 16px;
  overflow: hidden;
}

.calendar-weekdays {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  background: rgba(var(--v-theme-primary), 0.05);
  border-bottom: 1px solid rgba(var(--v-theme-outline), 0.2);
}

.weekday-header {
  padding: 16px 8px;
  text-align: center;
  font-weight: 600;
  color: rgb(var(--v-theme-primary));
  font-size: 14px;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  min-height: 600px;
}

.calendar-day {
  border-right: 1px solid rgba(var(--v-theme-outline), 0.1);
  border-bottom: 1px solid rgba(var(--v-theme-outline), 0.1);
  padding: 12px 8px;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
  min-height: 80px;
  display: flex;
  flex-direction: column;
}

.calendar-day:hover {
  background: rgba(var(--v-theme-primary), 0.05);
}

.calendar-day--other-month {
  opacity: 0.3;
  background: rgba(var(--v-theme-surface), 0.5);
}

.calendar-day--today {
  background: rgba(var(--v-theme-primary), 0.1);
  font-weight: 600;
}

.calendar-day--today::before {
  content: '';
  position: absolute;
  top: 4px;
  right: 4px;
  width: 8px;
  height: 8px;
  background: rgb(var(--v-theme-primary));
  border-radius: 50%;
}

.calendar-day--selected {
  background: rgba(var(--v-theme-primary), 0.15);
  border: 2px solid rgb(var(--v-theme-primary));
}

.calendar-day--has-transactions {
  background: rgba(var(--v-theme-primary), 0.03);
}

.day-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.day-number {
  font-size: 14px;
  font-weight: 500;
}

.transaction-count {
  background: rgb(var(--v-theme-primary));
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 600;
}

.transaction-indicators {
  display: flex;
  flex-wrap: wrap;
  gap: 2px;
  margin-top: auto;
}

.transaction-indicator {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  flex-shrink: 0;
}

.more-indicator {
  font-size: 8px;
  color: rgb(var(--v-theme-on-surface-variant));
  font-weight: 500;
  padding: 2px 4px;
  background: rgba(var(--v-theme-surface), 0.8);
  border-radius: 4px;
}

.transaction-item {
  border-radius: 8px;
  margin-bottom: 8px;
  transition: all 0.2s ease;
}

.transaction-item:hover {
  background: rgba(var(--v-theme-primary), 0.05);
}

.container-custom {
  max-width: 1200px;
  margin: 0 auto;
}

.main-content {
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.02) 0%, rgba(var(--v-theme-secondary), 0.02) 100%);
  min-height: 100vh;
}

.transaction-list {
  max-height: 600px;
  overflow-y: auto;
  padding-right: 8px;
}

.transaction-card {
  transition: all 0.3s ease;
  border: 1px solid rgba(var(--v-theme-outline), 0.2);
  background: rgba(var(--v-theme-surface), 0.5);
}

.transaction-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(var(--v-theme-primary), 0.15);
  border-color: rgba(var(--v-theme-primary), 0.3);
}

.transaction-card .v-card-text {
  padding: 20px !important;
}

/* Custom scrollbar for transaction list */
.transaction-list::-webkit-scrollbar {
  width: 6px;
}

.transaction-list::-webkit-scrollbar-track {
  background: rgba(var(--v-theme-outline), 0.1);
  border-radius: 3px;
}

.transaction-list::-webkit-scrollbar-thumb {
  background: rgba(var(--v-theme-primary), 0.3);
  border-radius: 3px;
}

.transaction-list::-webkit-scrollbar-thumb:hover {
  background: rgba(var(--v-theme-primary), 0.5);
}
</style>
