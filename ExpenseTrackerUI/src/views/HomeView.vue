<template>
  <v-app>
    <AppHeader />
    <AppNav />
    
    <v-main class="main-content">
      <div class="container-custom pa-3 pa-sm-6">
        <!-- Welcome Section -->
        <div 
          class="welcome-section mb-6 mb-sm-8"
          v-motion
          :initial="{ opacity: 0, y: -20 }"
          :enter="{ opacity: 1, y: 0, transition: { duration: 500 } }"
        >
          <div class="welcome-content">
            <div class="welcome-text">
              <h1 class="welcome-title text-h4 text-sm-h3 font-weight-bold mb-2">
                Welcome back, Han! 👋
              </h1>
              <p class="welcome-subtitle text-subtitle-1 text-sm-h6 text-secondary mb-3 mb-sm-0">
                Here's what's happening with your finances today.
              </p>
            </div>
            
            <!-- Quick Actions -->
            <div class="quick-actions d-flex flex-column flex-sm-row align-stretch align-sm-center gap-2">
              <v-btn
                color="primary"
                variant="elevated"
                :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-plus'"
                :size="$vuetify.display.mobile ? 'small' : 'default'"
                @click="showAddTransaction = true"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-plus</v-icon>
                Add Transaction
              </v-btn>
              
              <v-btn
                color="success"
                variant="tonal"
                :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-target'"
                :size="$vuetify.display.mobile ? 'small' : 'default'"
                @click="showAddGoal = true"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-target</v-icon>
                New Goal
              </v-btn>
            </div>
          </div>
          
          <!-- Quick Stats Bar -->
          <div class="quick-stats-bar mt-6">
            <v-row>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Today's Spending</p>
                  <p class="text-h6 font-weight-bold text-primary mb-0">{{ formatCurrency(247.32) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Weekly Budget Left</p>
                  <p class="text-h6 font-weight-bold text-success mb-0">{{ formatCurrency(1652.68) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Goals Progress</p>
                  <p class="text-h6 font-weight-bold text-warning mb-0">73%</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Credit Score</p>
                  <p class="text-h6 font-weight-bold text-info mb-0">742</p>
                </div>
              </v-col>
            </v-row>
          </div>
        </div>

        <!-- Summary Cards -->
        <div 
          class="summary-section mb-8"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 200, duration: 500 } }"
        >
          <SummaryCards />
        </div>

        <!-- Charts and Goals Row -->
        <div 
          class="analytics-section mb-8"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 400, duration: 500 } }"
        >
          <v-row>
            <v-col cols="12" lg="8">
              <MiniChart />
            </v-col>
            <v-col cols="12" lg="4">
              <GoalsList />
            </v-col>
          </v-row>
        </div>

        <!-- Calendar Panel -->
        <div 
          class="calendar-section mb-8"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 500, duration: 500 } }"
        >
          <v-row>
            <v-col cols="12" md="6">
              <CalendarPanel />
            </v-col>
            <v-col cols="12" md="6">
              <!-- Placeholder for second panel -->
              <v-card class="dashboard-card glass-card" style="height: 400px;">
                <v-card-text class="d-flex align-center justify-center h-100">
                  <div class="text-center">
                    <v-icon icon="mdi-plus" size="48" color="primary" class="mb-4"></v-icon>
                    <h3 class="text-h6 mb-2">Add Another Panel</h3>
                    <p class="text-body-2 text-secondary">Space for additional dashboard content</p>
                  </div>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </div>

        <!-- Recent Transactions -->
        <div 
          class="transactions-section"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 600, duration: 500 } }"
        >
          <RecentTransactions />
        </div>

        <!-- Floating Action Button (Mobile) -->
        <v-fab
          v-if="isMobile"
          color="primary"
          icon="mdi-plus"
          location="bottom end"
          size="large"
          @click="showQuickActions = true"
          class="fab-main"
        />
      </div>
    </v-main>

    <AppFooter />

    <!-- Quick Action Dialogs -->
    <v-dialog v-model="showAddTransaction" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-plus" class="mr-2"></v-icon>
          Add Transaction
        </v-card-title>
        <v-card-text>
          <v-form ref="formRef" v-model="formValid">
            <v-row>
              <v-col cols="12" md="6">
                <v-select
                  v-model="transactionForm.accountId"
                  :items="accountOptions"
                  label="Account"
                  :rules="accountRules"
                  variant="outlined"
                  rounded="xl"
                  class="mb-3"
                ></v-select>
              </v-col>
              <v-col cols="12" md="6">
                <v-select
                  v-model="transactionForm.categoryId"
                  :items="categoryOptions"
                  label="Category"
                  :rules="categoryRules"
                  variant="outlined"
                  rounded="xl"
                  class="mb-3"
                  @update:model-value="onCategoryChange"
                ></v-select>
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <v-select
                  v-model="transactionForm.subCategoryId"
                  :items="subCategoryOptions"
                  label="Subcategory"
                  :rules="subCategoryRules"
                  variant="outlined"
                  rounded="xl"
                  class="mb-3"
                ></v-select>
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="transactionForm.amount"
                  label="Amount"
                  type="number"
                  step="0.01"
                  min="0"
                  :rules="amountRules"
                  variant="outlined"
                  rounded="xl"
                  class="mb-3"
                ></v-text-field>
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="transactionForm.transactionDate"
                  label="Transaction Date"
                  type="date"
                  :rules="dateRules"
                  variant="outlined"
                  rounded="xl"
                  class="mb-3"
                ></v-text-field>
              </v-col>
            </v-row>

            <v-textarea
              v-model="transactionForm.description"
              label="Description (Optional)"
              variant="outlined"
              rounded="xl"
              rows="3"
            ></v-textarea>
          </v-form>
        </v-card-text>
        <v-card-actions class="px-6 pb-4">
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="closeTransactionDialog"
            rounded="xl"
            class="text-capitalize"
          >
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            variant="flat"
            @click="saveTransaction"
            :loading="saving"
            :disabled="!formValid"
            rounded="xl"
            class="text-capitalize"
          >
            Add Transaction
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="showAddGoal" max-width="600">
      <v-card>
        <v-card-title>Create New Goal</v-card-title>
        <v-card-text>
          <p>Goal creation form would go here...</p>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="showAddGoal = false">Cancel</v-btn>
          <v-btn color="primary" @click="showAddGoal = false">Create</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Mobile Floating Action Button -->
    <div v-if="$vuetify.display.mobile" class="fab-container">
      <v-menu
        v-model="showQuickActions"
        :close-on-content-click="false"
        location="top"
        origin="bottom"
      >
        <template v-slot:activator="{ props }">
          <v-btn
            v-bind="props"
            color="primary"
            icon
            size="large"
            elevation="8"
            class="fab-main"
          >
            <v-icon :class="{ 'rotate-45': showQuickActions }">
              {{ showQuickActions ? 'mdi-close' : 'mdi-plus' }}
            </v-icon>
          </v-btn>
        </template>
        
        <v-card class="fab-menu" elevation="8">
          <v-list density="compact">
            <v-list-item
              prepend-icon="mdi-plus"
              title="Add Transaction"
              @click="showAddTransaction = true; showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-target"
              title="New Goal"
              @click="showAddGoal = true; showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-chart-line"
              title="View Reports"
              @click="$router.push('/reports'); showQuickActions = false"
            />
          </v-list>
        </v-card>
      </v-menu>
    </div>

    <!-- Mobile Quick Actions (Alternative) -->
    <v-bottom-sheet v-model="showQuickActions">
      <v-card>
        <v-card-title>Quick Actions</v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="6">
              <v-btn
                block
                color="primary"
                prepend-icon="mdi-plus"
                @click="showAddTransaction = true; showQuickActions = false"
              >
                Add Transaction
              </v-btn>
            </v-col>
            <v-col cols="6">
              <v-btn
                block
                color="success"
                prepend-icon="mdi-target"
                @click="showAddGoal = true; showQuickActions = false"
              >
                New Goal
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-bottom-sheet>
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { useDisplay } from 'vuetify'
import { formatCurrency } from '@/utils/formatters'
import { transactionApi, accountApi, categoryApi } from '@/lib/api'
import type { CreateTransactionDto } from '@/types'
import type { AccountDto } from '@/types/account'
import type { CategoryDto } from '@/types/category'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import SummaryCards from '@/components/Dashboard/SummaryCards.vue'
import MiniChart from '@/components/Dashboard/MiniChart.vue'
import GoalsList from '@/components/Dashboard/GoalsList.vue'
import RecentTransactions from '@/components/Dashboard/RecentTransactions.vue'
import CalendarPanel from '@/components/Dashboard/CalendarPanel.vue'

const { mobile } = useDisplay()

// Reactive state
const showAddTransaction = ref(false)
const showAddGoal = ref(false)
const showQuickActions = ref(false)
const saving = ref(false)
const formValid = ref(false)

// Transaction form
const transactionForm = reactive<CreateTransactionDto>({
  accountId: '',
  categoryId: '',
  subCategoryId: '',
  description: '',
  amount: 0,
  transactionDate: new Date().toISOString().split('T')[0]
})

// Data
const accounts = ref<AccountDto[]>([])
const categories = ref<CategoryDto[]>([])

// Computed properties
const isMobile = computed(() => mobile.value)

const accountOptions = computed(() => 
  accounts.value.map(account => ({
    title: account.name,
    value: account.id
  }))
)

const categoryOptions = computed(() => 
  categories.value.map(category => ({
    title: category.name,
    value: category.id
  }))
)

const subCategoryOptions = computed(() => {
  if (!transactionForm.categoryId) return []
  const category = categories.value.find(c => c.id === transactionForm.categoryId)
  if (!category?.subCategories) return []
  
  return category.subCategories.map(sub => ({
    title: sub.name,
    value: sub.id
  }))
})

// Form rules
const accountRules = [(v: string) => !!v || 'Account is required']
const categoryRules = [(v: string) => !!v || 'Category is required']
const subCategoryRules = [(v: string) => !!v || 'Subcategory is required']
const amountRules = [
  (v: number) => !!v || 'Amount is required',
  (v: number) => v > 0 || 'Amount must be positive'
]
const dateRules = [(v: string) => !!v || 'Date is required']

// Methods
const loadAccounts = async () => {
  try {
    const data = await accountApi.list()
    accounts.value = data || []
  } catch (error) {
    console.error('Failed to load accounts:', error)
    accounts.value = []
  }
}

const loadCategories = async () => {
  try {
    const data = await categoryApi.list()
    categories.value = data || []
  } catch (error) {
    console.error('Failed to load categories:', error)
    categories.value = []
  }
}

const closeTransactionDialog = () => {
  showAddTransaction.value = false
  // Reset form
  transactionForm.accountId = ''
  transactionForm.categoryId = ''
  transactionForm.subCategoryId = ''
  transactionForm.description = ''
  transactionForm.amount = 0
  transactionForm.transactionDate = new Date().toISOString().split('T')[0]
}

const saveTransaction = async () => {
  if (!formValid.value) return

  saving.value = true
  try {
    await transactionApi.create(transactionForm)
    closeTransactionDialog()
    // TODO: Show success notification
    // TODO: Refresh recent transactions
  } catch (error) {
    console.error('Failed to save transaction:', error)
    // TODO: Show error notification
  } finally {
    saving.value = false
  }
}

const onCategoryChange = () => {
  transactionForm.subCategoryId = ''
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadAccounts(),
    loadCategories()
  ])
})
</script>
