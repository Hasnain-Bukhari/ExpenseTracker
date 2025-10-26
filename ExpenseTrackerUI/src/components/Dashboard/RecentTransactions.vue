<template>
  <v-card class="transactions-card dashboard-card">
    <!-- Card Header -->
    <v-card-title class="transactions-header pa-4 pa-sm-6 pb-4">
      <div class="header-content w-100">
        <div class="header-main d-flex align-center">
          <v-avatar 
            :size="$vuetify.display.mobile ? 36 : 40" 
            color="primary" 
            variant="tonal"
            class="mr-3"
          >
            <v-icon icon="mdi-history" :size="$vuetify.display.mobile ? 18 : 20" />
          </v-avatar>
          <div class="header-text">
            <h3 class="text-h6 text-sm-h5 font-weight-bold mb-0">Recent Transactions</h3>
            <p class="text-caption text-sm-body-2 text-secondary mb-0 d-none d-sm-block">
              Last {{ displayLimit }} transactions
            </p>
          </div>
        </div>
        
        <!-- Header Actions -->
        <div class="header-actions d-flex align-center mt-3 mt-sm-0">
          <v-btn-toggle
            v-model="viewMode"
            variant="outlined"
            :size="$vuetify.display.mobile ? 'x-small' : 'small'"
            density="compact"
            class="mr-2 mr-sm-3"
          >
            <v-btn value="list" :size="$vuetify.display.mobile ? 'x-small' : 'small'">
              <v-icon>mdi-view-list</v-icon>
            </v-btn>
            <v-btn value="card" :size="$vuetify.display.mobile ? 'x-small' : 'small'">
              <v-icon>mdi-view-grid</v-icon>
            </v-btn>
          </v-btn-toggle>
          
          <v-btn
            variant="text"
            :size="$vuetify.display.mobile ? 'x-small' : 'small'"
            color="primary"
            @click="viewAllTransactions"
            :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-arrow-right'"
          >
            <span class="d-none d-sm-inline">View All</span>
            <v-icon class="d-sm-none">mdi-arrow-right</v-icon>
          </v-btn>
        </div>
      </div>
    </v-card-title>

    <!-- Transactions Content -->
    <v-card-text class="pa-0">
      <!-- Loading State -->
      <div v-if="isLoading" class="loading-container pa-6">
        <div v-for="i in 5" :key="i" class="skeleton-transaction mb-3">
          <div class="d-flex align-center">
            <div class="skeleton skeleton-avatar mr-3" />
            <div class="flex-grow-1">
              <div class="skeleton skeleton-text mb-2" style="width: 60%;" />
              <div class="skeleton skeleton-text" style="width: 40%;" />
            </div>
            <div class="skeleton skeleton-text" style="width: 80px;" />
          </div>
        </div>
      </div>

      <!-- List View -->
      <div v-else-if="viewMode === 'list'" class="transactions-list">
        <v-table class="transactions-table">
          <thead class="d-none d-sm-table-header-group">
            <tr class="table-header">
              <th class="text-left font-weight-semibold">Transaction</th>
              <th class="text-left font-weight-semibold d-none d-md-table-cell">Category</th>
              <th class="text-left font-weight-semibold d-none d-lg-table-cell">Account</th>
              <th class="text-right font-weight-semibold">Amount</th>
              <th class="text-center font-weight-semibold d-none d-md-table-cell">Status</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(transaction, index) in recentTransactions"
              :key="transaction.id"
              class="transaction-row"
              v-motion
              :initial="{ opacity: 0, x: -20 }"
              :enter="{ 
                opacity: 1, 
                x: 0, 
                transition: { 
                  delay: index * 50,
                  type: 'spring',
                  stiffness: 200
                } 
              }"
            >
              <!-- Transaction Info -->
              <td class="transaction-info">
                <div class="d-flex align-center">
                  <v-avatar
                    :size="$vuetify.display.mobile ? 32 : 36"
                    :color="getCategoryColor(transaction)"
                    variant="tonal"
                    class="mr-3"
                  >
                    <v-icon 
                      :icon="getCategoryIcon(transaction)"
                      :size="$vuetify.display.mobile ? 16 : 18"
                    />
                  </v-avatar>
                  <div class="transaction-details">
                    <p class="text-body-2 text-sm-body-1 font-weight-medium mb-0 text-truncate">
                      {{ transaction.description || 'No description' }}
                    </p>
                    <div class="transaction-meta d-flex flex-column flex-sm-row align-start align-sm-center">
                      <span class="text-caption text-secondary">
                        {{ formatDateShort(transaction.transactionDate) }}
                      </span>
                      <!-- Show category and account on mobile -->
                      <div class="d-flex d-md-none align-center mt-1 mt-sm-0 ml-sm-2">
                        <v-chip
                          size="x-small"
                          variant="tonal"
                          :color="getCategoryColor(transaction)"
                          class="mr-2"
                        >
                          {{ transaction.category?.name || 'Unknown' }}
                        </v-chip>
                        <span class="text-xs text-secondary d-lg-none">
                          {{ transaction.account?.name || 'Unknown' }}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </td>
              
              <!-- Category -->
              <td class="d-none d-md-table-cell">
                <v-chip
                  size="small"
                  variant="tonal"
                  :color="getCategoryColor(transaction)"
                  class="category-chip"
                >
                  {{ transaction.category?.name || 'Unknown' }}
                </v-chip>
              </td>
              
              <!-- Account -->
              <td class="d-none d-lg-table-cell">
                <div class="d-flex align-center">
                  <v-icon 
                    :icon="getAccountIcon(transaction.account)"
                    size="16"
                    class="mr-2 text-secondary"
                  />
                  <span class="text-body-2">{{ transaction.account?.name || 'Unknown' }}</span>
                </div>
              </td>
              
              <!-- Amount -->
              <td class="text-right">
                <div class="amount-container">
                  <span 
                    :class="[
                      'amount text-body-1 font-weight-bold text-currency',
                      getAmountClass(transaction)
                    ]"
                  >
                    {{ getAmountPrefix(transaction) }}{{ formatCurrency(transaction.amount) }}
                  </span>
                </div>
              </td>
              
              <!-- Status -->
              <td class="text-center d-none d-md-table-cell">
                <v-chip
                  size="small"
                  :color="getStatusColor()"
                  variant="tonal"
                  class="status-chip"
                >
                  Completed
                </v-chip>
              </td>
            </tr>
          </tbody>
        </v-table>
      </div>

      <!-- Card View -->
      <div v-else class="transactions-cards pa-2 pa-sm-4">
        <v-row :class="{ 'ma-0': $vuetify.display.mobile }">
          <v-col
            v-for="(transaction, index) in recentTransactions"
            :key="transaction.id"
            cols="12"
            sm="6"
            lg="4"
            :class="{ 'pa-1': $vuetify.display.mobile, 'pa-2': !$vuetify.display.mobile }"
          >
            <v-card
              class="transaction-card hover-lift"
              variant="outlined"
              v-motion
              :initial="{ opacity: 0, scale: 0.9 }"
              :enter="{ 
                opacity: 1, 
                scale: 1,
                transition: { 
                  delay: index * 100,
                  type: 'spring'
                } 
              }"
            >
              <v-card-text class="pa-3 pa-sm-4">
                <div class="d-flex align-center justify-space-between mb-3">
                  <v-avatar
                    :color="getCategoryColor(transaction)"
                    variant="tonal"
                    :size="$vuetify.display.mobile ? 36 : 40"
                  >
                    <v-icon 
                      :icon="getCategoryIcon(transaction)" 
                      :size="$vuetify.display.mobile ? 18 : 20" 
                    />
                  </v-avatar>
                  
                  <v-chip
                    :color="getStatusColor()"
                    :size="$vuetify.display.mobile ? 'x-small' : 'small'"
                    variant="tonal"
                  >
                    Completed
                  </v-chip>
                </div>
                
                <h4 class="text-subtitle-2 text-sm-subtitle-1 font-weight-semibold mb-1 text-truncate">
                  {{ transaction.description || 'No description' }}
                </h4>
                
                <p class="text-caption text-sm-body-2 text-secondary mb-2">
                  <span class="category-text">{{ transaction.category?.name || 'Unknown' }}</span>
                  <span class="d-none d-sm-inline"> • </span>
                  <span class="d-block d-sm-inline text-xs">{{ formatDateShort(transaction.transactionDate) }}</span>
                </p>
                
                <div class="d-flex align-center justify-space-between flex-wrap">
                  <span class="text-caption text-sm-body-2 text-secondary mb-1 mb-sm-0">
                    {{ transaction.account?.name || 'Unknown' }}
                  </span>
                  
                  <span 
                    :class="[
                      'text-subtitle-1 text-sm-h6 font-weight-bold text-currency',
                      getAmountClass(transaction)
                    ]"
                  >
                    {{ getAmountPrefix(transaction) }}{{ formatCurrency(transaction.amount) }}
                  </span>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </div>

      <!-- Load More Button -->
      <div v-if="!isLoading && hasMore" class="load-more-container pa-4 text-center">
        <v-btn
          variant="outlined"
          color="primary"
          @click="loadMore"
          :loading="isLoadingMore"
        >
          <v-icon start>mdi-refresh</v-icon>
          Load More Transactions
        </v-btn>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useDisplay } from 'vuetify'
import { useToast } from 'vue-toastification'
import { formatCurrency, formatDateShort } from '@/utils/formatters'
import { transactionService } from '@/services/apiService'
import type { TransactionDto } from '@/types/transaction'
import { CategoryType } from '@/types/category'

const router = useRouter()
const toast = useToast()
const { mobile } = useDisplay()

// Reactive state
const viewMode = ref<'list' | 'card'>('list')
const displayLimit = ref(10)
const isLoading = ref(false)
const isLoadingMore = ref(false)
const transactions = ref<TransactionDto[]>([])
const hasMoreData = ref(false)

// Auto-switch to card view on mobile
watch(mobile, (isMobile) => {
  if (isMobile) {
    viewMode.value = 'card'
  }
}, { immediate: true })

// Computed properties
const recentTransactions = computed(() => 
  transactions.value.slice(0, displayLimit.value)
)

const hasMore = computed(() => 
  hasMoreData.value && transactions.value.length > displayLimit.value
)

// Data loading
const loadRecentTransactions = async () => {
  isLoading.value = true
  try {
    const response = await transactionService.list({ 
      page: 1, 
      pageSize: 20 // Load more than display limit to enable "load more"
    })
    
    // Handle both direct array and paged result
    if (Array.isArray(response)) {
      transactions.value = response
      hasMoreData.value = response.length >= 20
    } else if (response && response.items) {
      transactions.value = response.items
      hasMoreData.value = response.total > response.items.length
    } else {
      transactions.value = []
      hasMoreData.value = false
    }
  } catch (error: any) {
    console.error('Failed to load recent transactions:', error)
    toast.error('Failed to load recent transactions')
    transactions.value = []
    hasMoreData.value = false
  } finally {
    isLoading.value = false
  }
}

// Styling helpers
const getAmountClass = (transaction: TransactionDto): string => {
  switch (transaction.category?.categoryType) {
    case CategoryType.Income: return 'text-success'
    case CategoryType.Expense: return 'text-error'
    case CategoryType.TargetedSavingsGoal: return 'text-info'
    default: return 'text-medium-emphasis'
  }
}

const getAmountPrefix = (transaction: TransactionDto): string => {
  switch (transaction.category?.categoryType) {
    case CategoryType.Income: return '+'
    case CategoryType.Expense: return '-'
    case CategoryType.TargetedSavingsGoal: return '→'
    default: return ''
  }
}

const getStatusColor = (): string => {
  return 'success' // All transactions are considered completed
}

const getCategoryColor = (transaction: TransactionDto): string => {
  switch (transaction.category?.categoryType) {
    case CategoryType.Income: return 'success'
    case CategoryType.Expense: return 'error'
    case CategoryType.TargetedSavingsGoal: return 'info'
    default: return 'default'
  }
}

const getCategoryIcon = (transaction: TransactionDto): string => {
  switch (transaction.category?.categoryType) {
    case CategoryType.Income: return 'mdi-trending-up'
    case CategoryType.Expense: return 'mdi-trending-down'
    case CategoryType.TargetedSavingsGoal: return 'mdi-target'
    default: return 'mdi-help-circle'
  }
}

const getAccountIcon = (account: any): string => {
  if (!account) return 'mdi-bank-outline'
  if (account.name?.toLowerCase().includes('credit')) return 'mdi-credit-card-outline'
  if (account.name?.toLowerCase().includes('savings')) return 'mdi-piggy-bank-outline'
  return 'mdi-bank-outline'
}

// Actions
const viewAllTransactions = () => {
  router.push('/transactions')
}

const loadMore = async () => {
  isLoadingMore.value = true
  
  try {
    // Increase display limit to show more transactions
    displayLimit.value += 5
  } catch (error) {
    console.error('Failed to load more transactions:', error)
    toast.error('Failed to load more transactions')
  } finally {
    isLoadingMore.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadRecentTransactions()
})
</script>
