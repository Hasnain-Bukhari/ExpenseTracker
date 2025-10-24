<template>
  <v-card class="dashboard-card glass-card" v-motion :initial="{ opacity: 0, y: 20 }" :enter="{ opacity: 1, y: 0, transition: { delay: 200, duration: 500 } }">
    <!-- Card Background Pattern -->
    <div class="card-pattern" />
    
    <!-- Loading State -->
    <div v-if="isLoading" class="loading-overlay">
      <div class="skeleton skeleton-card" />
    </div>
    
    <!-- Card Content -->
    <v-card-text v-else class="pa-4 pa-sm-6">
      <!-- Header Section -->
      <div class="d-flex align-center justify-space-between mb-4">
        <div class="d-flex align-center">
          <v-avatar size="48" class="card-icon card-icon--primary mr-3">
            <v-icon icon="mdi-calendar-month" color="primary" size="24" />
          </v-avatar>
          <div>
            <h3 class="card-title text-h6 font-weight-medium mb-1">Transaction Calendar</h3>
            <p class="text-caption text-secondary mb-0">View transactions by date</p>
          </div>
        </div>
        
        <!-- View Full Calendar Button -->
        <v-btn
          color="primary"
          variant="text"
          size="small"
          @click="navigateToCalendar"
          class="text-capitalize"
        >
          View Full Calendar
          <v-icon end size="14">mdi-arrow-right</v-icon>
        </v-btn>
      </div>
      
      <!-- Mini Calendar Grid -->
      <div class="mini-calendar">
        <!-- Calendar Header -->
        <div class="calendar-header">
          <div class="d-flex align-center justify-space-between mb-4">
            <div>
              <h4 class="text-subtitle-1 font-weight-medium mb-1">{{ currentMonthYear }}</h4>
              <p class="text-caption text-secondary mb-0">{{ monthlyTransactionCount }} transactions this month</p>
            </div>
            <div class="d-flex gap-1">
              <v-btn
                icon
                size="small"
                variant="text"
                @click="previousMonth"
                :disabled="isLoading"
                class="nav-btn"
              >
                <v-icon size="16">mdi-chevron-left</v-icon>
              </v-btn>
              <v-btn
                icon
                size="small"
                variant="text"
                @click="nextMonth"
                :disabled="isLoading"
                class="nav-btn"
              >
                <v-icon size="16">mdi-chevron-right</v-icon>
              </v-btn>
            </div>
          </div>
        </div>
        
        <!-- Calendar Grid -->
        <div class="calendar-grid">
          <!-- Weekday headers -->
          <div class="weekday-headers">
            <div v-for="day in weekdays" :key="day" class="weekday-header">
              {{ day }}
            </div>
          </div>
          
          <!-- Calendar days -->
          <div class="calendar-days">
            <div
              v-for="(day, index) in calendarDays"
              :key="index"
              :class="[
                'calendar-day',
                {
                  'calendar-day--other-month': !day.isCurrentMonth,
                  'calendar-day--today': day.isToday,
                  'calendar-day--has-transactions': day.transactionCount > 0
                }
              ]"
              @click="selectDate(day)"
            >
              <div class="day-number">{{ day.dayNumber }}</div>
              <div v-if="day.transactionCount > 0" class="transaction-indicator">
                <div
                  v-for="(transaction, txIndex) in day.transactions.slice(0, 3)"
                  :key="txIndex"
                  class="transaction-dot"
                  :style="{ backgroundColor: getCategoryTypeColor(transaction.category?.categoryType) }"
                ></div>
                <div v-if="day.transactionCount > 3" class="more-indicator">
                  +{{ day.transactionCount - 3 }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Summary Stats -->
      <div class="calendar-stats mt-4">
        <v-row>
          <v-col cols="4">
            <div class="stat-item text-center">
              <p class="text-caption text-secondary mb-1">This Month</p>
              <p class="text-body-2 font-weight-bold text-primary">{{ monthlyTransactionCount }}</p>
            </div>
          </v-col>
          <v-col cols="4">
            <div class="stat-item text-center">
              <p class="text-caption text-secondary mb-1">Total Amount</p>
              <p class="text-body-2 font-weight-bold text-success">{{ formatCurrency(monthlyTotalAmount) }}</p>
            </div>
          </v-col>
          <v-col cols="4">
            <div class="stat-item text-center">
              <p class="text-caption text-secondary mb-1">Avg/Day</p>
              <p class="text-body-2 font-weight-bold text-warning">{{ formatCurrency(dailyAverageAmount) }}</p>
            </div>
          </v-col>
        </v-row>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { formatCurrency } from '@/utils/formatters'
import { transactionService } from '@/services/apiService'
import type { TransactionDto } from '@/types/transaction'
import { CategoryType } from '@/types/category'

const router = useRouter()
const isLoading = ref(false)
const currentDate = ref(new Date())
const transactions = ref<TransactionDto[]>([])

// Calendar data
const weekdays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

// Helper functions
const getCategoryTypeColor = (categoryType: CategoryType): string => {
  return categoryType === CategoryType.Income ? '#4caf50' : '#f44336'
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
  
  console.log('CalendarPanel - Computing calendar days for:', month + 1, year)
  console.log('CalendarPanel - Total transactions available:', transactions.value.length)
  
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
  
  console.log('CalendarPanel - Generated', days.length, 'calendar days')
  const daysWithTransactions = days.filter(day => day.transactionCount > 0)
  console.log('CalendarPanel - Days with transactions:', daysWithTransactions.length)
  
  return days
})

const monthlyTransactionCount = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  
  return transactions.value.filter(tx => {
    const txDate = new Date(tx.transactionDate)
    return txDate.getFullYear() === year && txDate.getMonth() === month
  }).length
})

const monthlyTotalAmount = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  
  return transactions.value
    .filter(tx => {
      const txDate = new Date(tx.transactionDate)
      return txDate.getFullYear() === year && txDate.getMonth() === month
    })
    .reduce((sum, tx) => sum + tx.amount, 0)
})

const dailyAverageAmount = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  const daysInMonth = new Date(year, month + 1, 0).getDate()
  
  return monthlyTotalAmount.value / daysInMonth
})

// Methods
const getTransactionsForDate = (date: Date): TransactionDto[] => {
  const dateString = date.toISOString().split('T')[0]
  console.log(`CalendarPanel - Looking for date: ${dateString}`)
  console.log(`CalendarPanel - Available transaction dates:`, transactions.value.map(tx => tx.transactionDate).slice(0, 5))
  
  const dayTransactions = transactions.value.filter(tx => {
    // Try different date formats
    const txDate = tx.transactionDate
    const txDateOnly = txDate.split('T')[0] // Handle both "2025-10-24" and "2025-10-24T00:00:00Z"
    console.log(`CalendarPanel - Comparing ${dateString} with ${txDate} (${txDateOnly})`)
    return txDateOnly === dateString
  })
  
  console.log(`CalendarPanel - Date: ${dateString}, Found ${dayTransactions.length} transactions`)
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
    
    console.log('CalendarPanel - Loading transactions for month:', currentDate.value.getMonth() + 1, currentDate.value.getFullYear())
    console.log('CalendarPanel - Date range:', startOfMonth.toISOString().split('T')[0], 'to', endOfMonth.toISOString().split('T')[0])
    
    const response = await transactionService.list({ 
      page: 1, 
      pageSize: 1000,
      startDate: startOfMonth.toISOString().split('T')[0],
      endDate: endOfMonth.toISOString().split('T')[0]
    })
    console.log('Calendar Panel - Transaction response:', response)
    
    // Handle both direct array and paged result
    if (Array.isArray(response)) {
      transactions.value = response
    } else if (response && response.items) {
      transactions.value = response.items
    } else {
      transactions.value = []
    }
    
            console.log('Calendar Panel - Processed transactions:', transactions.value.length)
            console.log('Calendar Panel - Sample transaction:', transactions.value[0])
            console.log('Calendar Panel - Transaction dates:', transactions.value.map(tx => tx.transactionDate).slice(0, 5))
            console.log('Calendar Panel - First transaction date format:', transactions.value[0]?.transactionDate)
  } catch (error) {
    console.error('Failed to load transactions:', error)
    transactions.value = []
  } finally {
    isLoading.value = false
  }
}

const previousMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() - 1, 1)
  loadTransactions()
}

const nextMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 1)
  loadTransactions()
}

const selectDate = (day: any) => {
  if (day.transactionCount > 0) {
    // Navigate to calendar view with selected date
    router.push({
      name: 'calendar',
      query: { date: day.date.toISOString().split('T')[0] }
    })
  }
}

const navigateToCalendar = () => {
  router.push({ name: 'calendar' })
}

// Watch for month changes to reload data
watch(currentDate, () => {
  loadTransactions()
})

// Lifecycle
onMounted(() => {
  loadTransactions()
})
</script>

<style scoped>
.mini-calendar {
  background: rgba(var(--v-theme-surface), 0.8);
  border-radius: 20px;
  padding: 24px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(var(--v-theme-outline), 0.1);
  box-shadow: 0 8px 32px rgba(var(--v-theme-primary), 0.12);
  width: 100%;
  max-width: 100%;
  display: flex;
  flex-direction: column;
}

.calendar-header {
  margin-bottom: 20px;
  width: 100%;
}

.nav-btn {
  border-radius: 8px;
  transition: all 0.2s ease;
}

.nav-btn:hover {
  background: rgba(var(--v-theme-primary), 0.1);
  transform: scale(1.05);
}

.calendar-grid {
  display: flex;
  flex-direction: column;
  width: 100%;
  min-height: 240px;
}

.weekday-headers {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 6px;
  margin-bottom: 12px;
  width: 100%;
}

.weekday-header {
  text-align: center;
  font-size: 11px;
  font-weight: 600;
  color: rgb(var(--v-theme-primary));
  padding: 6px 4px;
  background: rgba(var(--v-theme-primary), 0.08);
  border-radius: 8px;
  text-transform: uppercase;
  letter-spacing: 0.3px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.calendar-days {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 6px;
  width: 100%;
}

.calendar-day {
  aspect-ratio: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  min-height: 36px;
  padding: 6px;
  background: rgba(var(--v-theme-surface), 0.6);
  border: 1px solid transparent;
  box-shadow: 0 1px 4px rgba(var(--v-theme-primary), 0.05);
}

.calendar-day:hover {
  background: rgba(var(--v-theme-primary), 0.15);
  border-color: rgba(var(--v-theme-primary), 0.3);
  transform: scale(1.08);
  box-shadow: 0 4px 16px rgba(var(--v-theme-primary), 0.2);
}

.calendar-day--other-month {
  opacity: 0.4;
  background: rgba(var(--v-theme-surface), 0.3);
  color: rgba(var(--v-theme-on-surface), 0.5);
}

.calendar-day--today {
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.25), rgba(var(--v-theme-primary), 0.15));
  border-color: rgb(var(--v-theme-primary));
  font-weight: 800;
  box-shadow: 0 4px 20px rgba(var(--v-theme-primary), 0.4);
  color: rgb(var(--v-theme-primary));
}

.calendar-day--has-transactions {
  background: rgba(var(--v-theme-primary), 0.12);
  border-color: rgba(var(--v-theme-primary), 0.3);
  box-shadow: 0 3px 12px rgba(var(--v-theme-primary), 0.15);
}

.day-number {
  font-size: 12px;
  font-weight: 600;
  margin-bottom: 2px;
  color: rgb(var(--v-theme-on-surface));
  line-height: 1;
}

.transaction-indicator {
  display: flex;
  align-items: center;
  gap: 3px;
  flex-wrap: wrap;
  justify-content: center;
  margin-top: 2px;
}

.transaction-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  flex-shrink: 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.more-indicator {
  font-size: 9px;
  color: rgb(var(--v-theme-on-surface-variant));
  font-weight: 700;
  padding: 2px 4px;
  background: rgba(var(--v-theme-surface), 0.95);
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
}

.calendar-stats {
  border-top: 2px solid rgba(var(--v-theme-outline), 0.15);
  padding-top: 24px;
  margin-top: 24px;
  background: rgba(var(--v-theme-primary), 0.03);
  border-radius: 16px;
  padding: 20px;
}

.stat-item {
  padding: 16px 12px;
  border-radius: 12px;
  transition: all 0.3s ease;
  background: rgba(var(--v-theme-surface), 0.8);
  border: 1px solid rgba(var(--v-theme-outline), 0.1);
}

.stat-item:hover {
  background: rgba(var(--v-theme-primary), 0.08);
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(var(--v-theme-primary), 0.15);
}

.card-pattern {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.05) 0%, transparent 50%);
  border-radius: inherit;
  pointer-events: none;
}

.loading-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(var(--v-theme-surface), 0.8);
  border-radius: inherit;
  z-index: 1;
}

.skeleton {
  background: linear-gradient(90deg, rgba(var(--v-theme-on-surface), 0.1) 25%, rgba(var(--v-theme-on-surface), 0.2) 50%, rgba(var(--v-theme-on-surface), 0.1) 75%);
  background-size: 200% 100%;
  animation: loading 1.5s infinite;
  border-radius: 8px;
}

.skeleton-card {
  width: 100%;
  height: 200px;
}

@keyframes loading {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

.dashboard-card {
  width: 100%;
  max-width: 100%;
  position: relative;
  overflow: hidden;
}
</style>
