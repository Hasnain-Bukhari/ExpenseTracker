<template>
  <v-app>
    <AppHeader @quick-add="handleQuickAdd" />
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
                @click="showAddGoalDialog = true"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-target</v-icon>
                New Goal
              </v-btn>
              
              <v-btn
                color="info"
                variant="tonal"
                :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-chart-line'"
                :size="$vuetify.display.mobile ? 'small' : 'default'"
                @click="showAddBudgetDialog = true"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-chart-line</v-icon>
                New Budget
              </v-btn>
            </div>
          </div>
          
          <!-- Quick Stats Bar -->
          <div class="quick-stats-bar mt-6">
            <v-row>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Today's Spending</p>
                  <p class="text-h6 font-weight-bold text-primary mb-0">{{ formatCurrency(todaySpending) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Monthly Budget Left</p>
                  <p class="text-h6 font-weight-bold text-success mb-0">{{ formatCurrency(monthlyBudgetRemaining) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Goals Progress</p>
                  <p class="text-h6 font-weight-bold text-warning mb-0">{{ Math.round(goalsProgressPercentage) }}%</p>
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
            <v-col cols="12" lg="6">
              <MiniChart />
            </v-col>
            <v-col cols="12" lg="3">
              <GoalsList />
            </v-col>
            <v-col cols="12" lg="3">
              <BudgetPanel />
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

    <!-- Add Goal Dialog -->
    <v-dialog v-model="showAddGoalDialog" max-width="600">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-target" class="mr-2"></v-icon>
          Create New Goal
        </v-card-title>
        
        <v-card-text>
          <v-form ref="goalFormRef" v-model="goalFormValid">
            <v-text-field
              v-model="goalForm.name"
              label="Goal Name"
              :rules="[v => !!v || 'Name is required']"
              variant="outlined"
              rounded="xl"
              required
              class="mb-4"
            />
            
            <v-textarea
              v-model="goalForm.description"
              label="Description"
              rows="3"
              variant="outlined"
              rounded="xl"
              class="mb-4"
            />
            
            <v-row>
              <v-col cols="6">
                <v-text-field
                  v-model.number="goalForm.targetAmount"
                  label="Target Amount"
                  type="number"
                  :rules="[v => v > 0 || 'Target amount must be greater than 0']"
                  variant="outlined"
                  rounded="xl"
                  required
                />
              </v-col>
              
              <v-col cols="6">
                <v-text-field
                  v-model.number="goalForm.currentAmount"
                  label="Current Amount"
                  type="number"
                  :rules="[v => v >= 0 || 'Current amount cannot be negative']"
                  variant="outlined"
                  rounded="xl"
                  required
                />
              </v-col>
            </v-row>
            
            <v-select
              v-model="goalForm.categoryId"
              :items="savingsCategories"
              item-title="name"
              item-value="id"
              label="Category"
              :rules="[v => !!v || 'Category is required']"
              variant="outlined"
              rounded="xl"
              required
              class="mb-4"
            />
            
            <v-row>
              <v-col cols="6">
                <v-text-field
                  v-model="goalForm.startDate"
                  label="Start Date"
                  type="date"
                  :rules="[v => !!v || 'Start date is required']"
                  variant="outlined"
                  rounded="xl"
                  required
                />
              </v-col>
              
              <v-col cols="6">
                <v-text-field
                  v-model="goalForm.endDate"
                  label="End Date (Optional)"
                  type="date"
                  variant="outlined"
                  rounded="xl"
                />
              </v-col>
            </v-row>
            
            <v-row>
              <v-col cols="6">
                <v-select
                  v-model="goalForm.priority"
                  :items="priorityOptions"
                  item-title="text"
                  item-value="value"
                  label="Priority"
                  variant="outlined"
                  rounded="xl"
                  required
                />
              </v-col>
              
              <v-col cols="6">
                <v-select
                  v-model="goalForm.status"
                  :items="statusOptions"
                  item-title="text"
                  item-value="value"
                  label="Status"
                  variant="outlined"
                  rounded="xl"
                  required
                />
              </v-col>
            </v-row>
            
            <v-text-field
              v-model="goalForm.tag"
              label="Tag (Optional)"
              placeholder="e.g., Emergency Fund, Vacation"
              variant="outlined"
              rounded="xl"
            />
          </v-form>
        </v-card-text>
        
        <v-card-actions>
          <v-spacer />
          <v-btn @click="closeGoalDialog" rounded="xl">Cancel</v-btn>
          <v-btn
            color="primary"
            :loading="savingGoal"
            :disabled="!goalFormValid"
            @click="saveGoal"
            rounded="xl"
          >
            Create
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Add Budget Dialog -->
    <v-dialog v-model="showAddBudgetDialog" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-chart-line" class="mr-2"></v-icon>
          Create Budget
        </v-card-title>
        
        <v-card-text>
          <v-form ref="budgetFormRef" v-model="budgetFormValid">
            <v-select
              v-model="budgetForm.categoryId"
              :items="expenseCategories"
              item-title="name"
              item-value="id"
              label="Category"
              :rules="[v => !!v || 'Category is required']"
              variant="outlined"
              rounded="xl"
              class="mb-4"
            />
            
            <v-text-field
              v-model.number="budgetForm.amount"
              label="Budget Amount"
              type="number"
              :rules="[
                v => !!v || 'Amount is required',
                v => v > 0 || 'Amount must be greater than zero'
              ]"
              variant="outlined"
              rounded="xl"
              prepend-inner-icon="mdi-currency-usd"
              min="0.01"
              step="0.01"
            />
          </v-form>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn @click="closeBudgetDialog" rounded="xl">Cancel</v-btn>
          <v-btn
            color="primary"
            @click="saveBudget"
            :loading="savingBudget"
            :disabled="!budgetFormValid"
            rounded="xl"
          >
            Create
          </v-btn>
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
              prepend-icon="mdi-plus-circle-outline"
              title="Add Transaction"
              @click="showAddTransaction = true; showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-target"
              title="New Goal"
              @click="showAddGoalDialog = true; showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-chart-line"
              title="New Budget"
              @click="showAddBudgetDialog = true; showQuickActions = false"
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
                  @click="showAddGoalDialog = true; showQuickActions = false"
                >
                  New Goal
                </v-btn>
              </v-col>
            </v-row>
            <v-row class="mt-2">
              <v-col cols="12">
                <v-btn
                  block
                  color="info"
                  prepend-icon="mdi-chart-line"
                  @click="showAddBudgetDialog = true; showQuickActions = false"
                >
                  New Budget
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
import { useToast } from 'vue-toastification'
import { formatCurrency } from '@/utils/formatters'
import { transactionApi, accountApi, categoryApi } from '@/lib/api'
import { goalService } from '@/lib/goalService'
import { budgetService } from '@/lib/budgetService'
import { categoryService } from '@/lib/categoryService'
import { dashboardService } from '@/lib/dashboardService'
import type { CreateTransactionDto } from '@/types'
import type { AccountDto } from '@/types/account'
import type { CategoryDto } from '@/types/category'
import { GoalStatus, GoalPriority } from '@/types/goal'
import type { CreateGoalDto } from '@/types/goal'
import type { CreateBudgetDto } from '@/types/budget'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import SummaryCards from '@/components/Dashboard/SummaryCards.vue'
import MiniChart from '@/components/Dashboard/MiniChart.vue'
import GoalsList from '@/components/Dashboard/GoalsList.vue'
import BudgetPanel from '@/components/Dashboard/BudgetPanel.vue'
import RecentTransactions from '@/components/Dashboard/RecentTransactions.vue'
import CalendarPanel from '@/components/Dashboard/CalendarPanel.vue'

const { mobile } = useDisplay()
const toast = useToast()

// Reactive state
const showAddTransaction = ref(false)
const showAddGoalDialog = ref(false)
const showAddBudgetDialog = ref(false)
const showQuickActions = ref(false)
const saving = ref(false)
const savingGoal = ref(false)
const savingBudget = ref(false)
const formValid = ref(false)
const goalFormValid = ref(false)
const budgetFormValid = ref(false)

// Goal form
const goalForm = reactive<CreateGoalDto>({
  name: '',
  description: '',
  targetAmount: 0,
  currentAmount: 0,
  categoryId: '',
  startDate: new Date().toISOString().split('T')[0],
  endDate: null,
  tag: '',
  status: GoalStatus.Active,
  priority: GoalPriority.Medium
})

// Budget form
const budgetForm = reactive({
  categoryId: '',
  amount: 0
})

// Goal options
const priorityOptions = [
  { text: 'Low', value: GoalPriority.Low },
  { text: 'Medium', value: GoalPriority.Medium },
  { text: 'High', value: GoalPriority.High }
]

const statusOptions = [
  { text: 'Active', value: GoalStatus.Active },
  { text: 'Paused', value: GoalStatus.Paused },
  { text: 'Completed', value: GoalStatus.Completed },
  { text: 'Cancelled', value: GoalStatus.Cancelled }
]

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
const savingsCategories = ref<CategoryDto[]>([])
const expenseCategories = ref<CategoryDto[]>([])

// Dashboard stats
const todaySpending = ref(0)
const monthlyBudgetRemaining = ref(0)
const goalsProgressPercentage = ref(0)

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

// Handle quick add from header
const handleQuickAdd = (action: 'transaction' | 'goal' | 'budget') => {
  if (action === 'transaction') {
    showAddTransaction.value = true
  } else if (action === 'goal') {
    showAddGoalDialog.value = true
  } else if (action === 'budget') {
    showAddBudgetDialog.value = true
  }
}

// Load savings categories for goals
const loadSavingsCategories = async () => {
  try {
    const allCategories = await categoryService.list()
    savingsCategories.value = allCategories.filter((cat: CategoryDto) => cat.categoryType === 'TargetedSavingsGoal')
  } catch (error) {
    console.error('Failed to load savings categories:', error)
    savingsCategories.value = []
  }
}

// Load expense categories for budgets
const loadExpenseCategories = async () => {
  try {
    expenseCategories.value = await categoryService.getExpenseCategories()
  } catch (error) {
    console.error('Failed to load expense categories:', error)
    expenseCategories.value = []
  }
}

// Close goal dialog
const closeGoalDialog = () => {
  showAddGoalDialog.value = false
  goalForm.name = ''
  goalForm.description = ''
  goalForm.targetAmount = 0
  goalForm.currentAmount = 0
  goalForm.categoryId = ''
  goalForm.startDate = new Date().toISOString().split('T')[0]
  goalForm.endDate = null
  goalForm.tag = ''
  goalForm.status = GoalStatus.Active
  goalForm.priority = GoalPriority.Medium
}

// Save goal
const saveGoal = async () => {
  if (!goalFormValid.value) return
  
  savingGoal.value = true
  try {
    await goalService.create(goalForm)
    toast.success('Goal created successfully')
    closeGoalDialog()
    // TODO: Refresh goals list
  } catch (error) {
    console.error('Failed to save goal:', error)
    toast.error('Failed to save goal')
  } finally {
    savingGoal.value = false
  }
}

// Close budget dialog
const closeBudgetDialog = () => {
  showAddBudgetDialog.value = false
  budgetForm.categoryId = ''
  budgetForm.amount = 0
}

// Save budget
const saveBudget = async () => {
  if (!budgetFormValid.value) return
  
  savingBudget.value = true
  try {
    const budgetData: CreateBudgetDto = {
      categoryId: budgetForm.categoryId,
      amount: budgetForm.amount
    }
    await budgetService.create(budgetData)
    toast.success('Budget created successfully')
    closeBudgetDialog()
    // TODO: Refresh budget list
  } catch (error) {
    console.error('Failed to save budget:', error)
    toast.error('Failed to save budget')
  } finally {
    savingBudget.value = false
  }
}

// Load dashboard stats
const loadDashboardStats = async () => {
  try {
    const stats = await dashboardService.getStats()
    todaySpending.value = stats.todaySpending
    monthlyBudgetRemaining.value = stats.monthlyBudgetRemaining
    goalsProgressPercentage.value = stats.goalsProgress.percentage
  } catch (error) {
    console.error('Failed to load dashboard stats:', error)
    // Keep default values of 0
  }
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadAccounts(),
    loadCategories(),
    loadSavingsCategories(),
    loadExpenseCategories(),
    loadDashboardStats()
  ])
})
</script>
