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
              <th class="text-center font-weight-semibold d-none d-sm-table-cell">Actions</th>
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
                    :color="getCategoryColor(transaction.category)"
                    variant="tonal"
                    class="mr-3"
                  >
                    <v-icon 
                      :icon="getCategoryIcon(transaction.category)"
                      :size="$vuetify.display.mobile ? 16 : 18"
                    />
                  </v-avatar>
                  <div class="transaction-details">
                    <p class="text-body-2 text-sm-body-1 font-weight-medium mb-0 text-truncate">
                      {{ transaction.description }}
                    </p>
                    <div class="transaction-meta d-flex flex-column flex-sm-row align-start align-sm-center">
                      <span class="text-caption text-secondary">
                        {{ formatDateShort(transaction.date) }}
                      </span>
                      <!-- Show category and account on mobile -->
                      <div class="d-flex d-md-none align-center mt-1 mt-sm-0 ml-sm-2">
                        <v-chip
                          size="x-small"
                          variant="tonal"
                          :color="getCategoryColor(transaction.category)"
                          class="mr-2"
                        >
                          {{ transaction.category }}
                        </v-chip>
                        <span class="text-xs text-secondary d-lg-none">
                          {{ transaction.account }}
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
                  :color="getCategoryColor(transaction.category)"
                  class="category-chip"
                >
                  {{ transaction.category }}
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
                  <span class="text-body-2">{{ transaction.account }}</span>
                </div>
              </td>
              
              <!-- Amount -->
              <td class="text-right">
                <div class="amount-container">
                  <span 
                    :class="[
                      'amount text-body-1 font-weight-bold text-currency',
                      getAmountClass(transaction.amount)
                    ]"
                  >
                    {{ getAmountPrefix(transaction.amount) }}{{ formatCurrency(Math.abs(transaction.amount)) }}
                  </span>
                </div>
              </td>
              
              <!-- Status -->
              <td class="text-center d-none d-md-table-cell">
                <v-chip
                  size="small"
                  :color="getStatusColor(transaction.status)"
                  variant="dot"
                  class="status-chip"
                >
                  {{ transaction.status }}
                </v-chip>
              </td>
              
              <!-- Actions -->
              <td class="text-center d-none d-sm-table-cell">
                <v-menu offset-y>
                  <template v-slot:activator="{ props }">
                    <v-btn
                      v-bind="props"
                      icon
                      size="small"
                      variant="text"
                      class="action-btn"
                    >
                      <v-icon size="16">mdi-dots-vertical</v-icon>
                    </v-btn>
                  </template>
                  <v-list density="compact">
                    <v-list-item @click="viewTransaction(transaction.id)">
                      <v-list-item-title>View Details</v-list-item-title>
                    </v-list-item>
                    <v-list-item @click="editTransaction(transaction.id)">
                      <v-list-item-title>Edit</v-list-item-title>
                    </v-list-item>
                    <v-list-item @click="duplicateTransaction(transaction.id)">
                      <v-list-item-title>Duplicate</v-list-item-title>
                    </v-list-item>
                  </v-list>
                </v-menu>
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
                    :color="getCategoryColor(transaction.category)"
                    variant="tonal"
                    :size="$vuetify.display.mobile ? 36 : 40"
                  >
                    <v-icon 
                      :icon="getCategoryIcon(transaction.category)" 
                      :size="$vuetify.display.mobile ? 18 : 20" 
                    />
                  </v-avatar>
                  
                  <v-chip
                    :color="getStatusColor(transaction.status)"
                    :size="$vuetify.display.mobile ? 'x-small' : 'small'"
                    variant="dot"
                  >
                    {{ transaction.status }}
                  </v-chip>
                </div>
                
                <h4 class="text-subtitle-2 text-sm-subtitle-1 font-weight-semibold mb-1 text-truncate">
                  {{ transaction.description }}
                </h4>
                
                <p class="text-caption text-sm-body-2 text-secondary mb-2">
                  <span class="category-text">{{ transaction.category }}</span>
                  <span class="d-none d-sm-inline"> • </span>
                  <span class="d-block d-sm-inline text-xs">{{ formatDateShort(transaction.date) }}</span>
                </p>
                
                <div class="d-flex align-center justify-space-between flex-wrap">
                  <span class="text-caption text-sm-body-2 text-secondary mb-1 mb-sm-0">
                    {{ transaction.account }}
                  </span>
                  
                  <span 
                    :class="[
                      'text-subtitle-1 text-sm-h6 font-weight-bold text-currency',
                      getAmountClass(transaction.amount)
                    ]"
                  >
                    {{ getAmountPrefix(transaction.amount) }}{{ formatCurrency(Math.abs(transaction.amount)) }}
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
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useDisplay } from 'vuetify'
import { useAppStore } from '@/stores'
import { formatCurrency, formatDateShort } from '@/utils/formatters'

const router = useRouter()
const store = useAppStore()
const { mobile } = useDisplay()

// Reactive state
const viewMode = ref<'list' | 'card'>('list')
const displayLimit = ref(8)
const isLoading = ref(false)
const isLoadingMore = ref(false)

// Auto-switch to card view on mobile
watch(mobile, (isMobile) => {
  if (isMobile) {
    viewMode.value = 'card'
  }
}, { immediate: true })

// Computed properties
const recentTransactions = computed(() => 
  store.recentTransactions.slice(0, displayLimit.value)
)

const hasMore = computed(() => 
  store.recentTransactions.length > displayLimit.value
)

// Styling helpers
const getAmountClass = (amount: number): string => {
  return amount >= 0 ? 'text-positive' : 'text-negative'
}

const getAmountPrefix = (amount: number): string => {
  return amount >= 0 ? '+' : '-'
}

const getStatusColor = (status: string): string => {
  switch (status) {
    case 'completed': return 'success'
    case 'pending': return 'warning'
    case 'cancelled': return 'error'
    default: return 'primary'
  }
}

const getCategoryColor = (category: string): string => {
  const colors: Record<string, string> = {
    'Groceries': 'green',
    'Dining': 'orange',
    'Transportation': 'blue',
    'Entertainment': 'purple',
    'Shopping': 'pink',
    'Utilities': 'teal',
    'Salary': 'success',
    'Transfer': 'info',
    'Gas': 'cyan',
    'Electric': 'amber',
  }
  return colors[category] || 'primary'
}

const getCategoryIcon = (category: string): string => {
  const icons: Record<string, string> = {
    'Groceries': 'mdi-cart-outline',
    'Dining': 'mdi-food-fork-drink',
    'Transportation': 'mdi-car-outline',
    'Entertainment': 'mdi-movie-outline',
    'Shopping': 'mdi-shopping-outline',
    'Utilities': 'mdi-lightning-bolt-outline',
    'Salary': 'mdi-cash-multiple',
    'Transfer': 'mdi-bank-transfer',
    'Gas': 'mdi-gas-station',
    'Electric': 'mdi-flash-outline',
  }
  return icons[category] || 'mdi-help-circle-outline'
}

const getAccountIcon = (account: string): string => {
  if (account.toLowerCase().includes('credit')) return 'mdi-credit-card-outline'
  if (account.toLowerCase().includes('savings')) return 'mdi-piggy-bank-outline'
  return 'mdi-bank-outline'
}

// Actions
const viewAllTransactions = () => {
  router.push('/transactions')
}

const viewTransaction = (id: string) => {
  router.push(`/transactions/${id}`)
}

const editTransaction = (id: string) => {
  router.push(`/transactions/${id}/edit`)
}

const duplicateTransaction = (id: string) => {
  // TODO: Implement duplicate functionality
  console.log('Duplicating transaction:', id)
}

const loadMore = async () => {
  isLoadingMore.value = true
  
  // Simulate loading delay
  setTimeout(() => {
    displayLimit.value += 5
    isLoadingMore.value = false
  }, 500)
}
</script>
