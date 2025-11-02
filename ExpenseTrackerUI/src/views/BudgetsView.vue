<template>
  <v-app>
    <AppHeader />
    <AppNav />
    
    <v-main>
      <v-container fluid class="pa-6">
        <!-- Header Section -->
        <div class="d-flex align-center justify-space-between mb-6">
          <div class="d-flex align-center">
            <v-avatar size="56" class="card-icon card-icon--primary mr-4">
              <v-icon icon="mdi-calculator-variant" color="primary" size="28" />
            </v-avatar>
            <div>
              <h1 class="text-h4 font-weight-bold text-primary mb-2">Budgets</h1>
              <p class="text-body-1 text-medium-emphasis mb-0">Manage your spending limits and track expenses</p>
            </div>
          </div>
          
          <v-btn
            color="primary"
            size="large"
            prepend-icon="mdi-plus"
            @click="openCreateDialog"
            rounded="xl"
            class="text-capitalize"
          >
            Create Budget
          </v-btn>
        </div>

        <!-- Tabs Section -->
        <v-card class="mb-6" rounded="xl">
          <v-tabs
            v-model="activeTab"
            color="primary"
            align-tabs="start"
            class="px-6"
          >
            <v-tab value="progress">
              <v-icon start>mdi-chart-pie</v-icon>
              Budget Progress
            </v-tab>
            <v-tab value="active">
              <v-icon start>mdi-chart-line</v-icon>
              Active Budgets
            </v-tab>
            <v-tab value="history">
              <v-icon start>mdi-history</v-icon>
              Budget History
            </v-tab>
          </v-tabs>

          <v-card-text class="pa-0">
            <!-- Budget Progress Tab -->
            <div v-if="activeTab === 'progress'">
              <div v-if="isLoading" class="pa-8 text-center">
                <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                <p class="mt-4 text-body-1">Loading budget progress...</p>
              </div>
              
              <div v-else-if="budgetProgress.length === 0" class="pa-8 text-center">
                <v-icon icon="mdi-chart-pie-outline" size="64" color="grey-lighten-1" class="mb-4" />
                <h3 class="text-h6 mb-2">No Active Budgets</h3>
                <p class="text-body-2 text-medium-emphasis mb-4">
                  Create your first budget to start tracking your spending progress.
                </p>
                <v-btn color="primary" variant="elevated" @click="openCreateDialog">
                  <v-icon start>mdi-plus</v-icon>
                  Create Budget
                </v-btn>
              </div>
              
              <div v-else class="pa-6">
                <v-row>
                  <v-col
                    v-for="budget in budgetProgress"
                    :key="budget.budgetId"
                    cols="12"
                    md="6"
                    lg="4"
                  >
                    <v-card
                      class="budget-card h-100"
                      elevation="2"
                      hover
                      @click="openEditDialog(budget)"
                    >
                      <v-card-text class="pa-6">
                        <div class="d-flex align-center justify-space-between mb-4">
                          <div class="d-flex align-center">
                            <v-avatar
                              :color="getBudgetStatusColor(budget)"
                              size="48"
                              class="mr-3"
                            >
                              <v-icon :color="getBudgetStatusIconColor()" size="24">
                                {{ getBudgetStatusIcon(budget) }}
                              </v-icon>
                            </v-avatar>
                            <div>
                              <h3 class="text-h6 font-weight-medium mb-1">
                                {{ budget.categoryName }}
                              </h3>
                              <p class="text-caption text-medium-emphasis mb-0">
                                Effective from {{ formatDate(budget.effectiveFrom) }}
                              </p>
                            </div>
                          </div>
                          <v-btn
                            icon
                            variant="text"
                            size="small"
                            @click.stop="openEditDialog(budget)"
                          >
                            <v-icon>mdi-pencil</v-icon>
                          </v-btn>
                        </div>
                        
                        <div class="mb-4">
                          <div class="d-flex justify-space-between align-center mb-2">
                            <span class="text-body-2 text-medium-emphasis">Budget Amount</span>
                            <span class="text-h6 font-weight-bold">${{ formatCurrency(budget.allocatedAmount) }}</span>
                          </div>
                          
                          <div class="d-flex justify-space-between align-center mb-2">
                            <span class="text-body-2 text-medium-emphasis">Spent Amount</span>
                            <span class="text-h6 font-weight-bold">${{ formatCurrency(budget.spentAmount) }}</span>
                          </div>
                          
                          <div class="d-flex justify-space-between align-center mb-2">
                            <span class="text-body-2 text-medium-emphasis">Remaining</span>
                            <span class="text-h6 font-weight-bold" :class="budget.remainingAmount < 0 ? 'text-error' : 'text-success'">
                              ${{ formatCurrency(budget.remainingAmount) }}
                            </span>
                          </div>
                          
                          <v-progress-linear
                            :model-value="getBudgetProgress(budget)"
                            :color="getBudgetStatusColor(budget)"
                            height="8"
                            rounded
                            class="mb-2"
                          ></v-progress-linear>
                          
                          <div class="d-flex justify-space-between text-caption text-medium-emphasis">
                            <span>{{ getBudgetProgress(budget) }}% used</span>
                            <span>{{ getBudgetStatusText(budget) }}</span>
                          </div>
                        </div>
                      </v-card-text>
                    </v-card>
                  </v-col>
                </v-row>
              </div>
            </div>

            <!-- Active Budgets Tab -->
            <div v-if="activeTab === 'active'">
              <div v-if="isLoading" class="pa-8 text-center">
                <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                <p class="mt-4 text-body-1">Loading active budgets...</p>
              </div>
              
              <div v-else-if="activeBudgets.length === 0" class="pa-8 text-center">
                <v-icon icon="mdi-chart-line-variant" size="64" color="grey-lighten-1" class="mb-4" />
                <h3 class="text-h6 mb-2">No Active Budgets</h3>
                <p class="text-body-2 text-medium-emphasis mb-4">Create your first budget to start tracking your expenses</p>
                <v-btn color="primary" @click="openCreateDialog" rounded="xl">
                  Create Budget
                </v-btn>
              </div>
              
              <div v-else class="pa-6">
                <v-data-table
                  :headers="activeBudgetHeaders"
                  :items="activeBudgets"
                  class="budget-table"
                  hide-default-footer
                >
                  <template v-slot:item.category="{ item }">
                    <div class="d-flex align-center">
                      <v-avatar
                        color="info"
                        size="32"
                        class="mr-3"
                      >
                        <v-icon color="white" size="16">
                          mdi-chart-line
                        </v-icon>
                      </v-avatar>
                      <span class="font-weight-medium">{{ item.category?.name || 'Unknown' }}</span>
                    </div>
                  </template>
                  
                  <template v-slot:item.amount="{ item }">
                    <span class="font-weight-bold">${{ formatCurrency(item.amount) }}</span>
                  </template>
                  
                  <template v-slot:item.effectiveFrom="{ item }">
                    {{ formatDate(item.effectiveFrom) }}
                  </template>
                  
                  <template v-slot:item.actions="{ item }">
                    <v-btn
                      icon
                      variant="text"
                      size="small"
                      @click="openEditDialogFromTable(item)"
                    >
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                  </template>
                </v-data-table>
              </div>
            </div>

            <!-- Budget History Tab -->
            <div v-if="activeTab === 'history'">
              <div v-if="isLoading" class="pa-8 text-center">
                <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                <p class="mt-4 text-body-1">Loading budget history...</p>
              </div>
              
              <div v-else-if="budgetHistory.length === 0" class="pa-8 text-center">
                <v-icon icon="mdi-history" size="64" color="grey-lighten-1" class="mb-4" />
                <h3 class="text-h6 mb-2">No Budget History</h3>
                <p class="text-body-2 text-medium-emphasis">Your completed budgets will appear here</p>
              </div>
              
              <div v-else class="pa-6">
                <v-data-table
                  :headers="historyHeaders"
                  :items="budgetHistory"
                  class="budget-history-table"
                  hide-default-footer
                >
                  <template v-slot:item.category="{ item }">
                    <div class="d-flex align-center">
                      <v-avatar
                        color="info"
                        size="32"
                        class="mr-3"
                      >
                        <v-icon color="white" size="16">
                          mdi-chart-line
                        </v-icon>
                      </v-avatar>
                      <span class="font-weight-medium">{{ item.category?.name || 'Unknown' }}</span>
                    </div>
                  </template>
                  
                  <template v-slot:item.amount="{ item }">
                    <span class="font-weight-bold">${{ formatCurrency(item.amount) }}</span>
                  </template>
                  
                  <template v-slot:item.effectiveFrom="{ item }">
                    {{ formatDate(item.effectiveFrom) }}
                  </template>
                  
                  <template v-slot:item.effectiveTo="{ item }">
                    {{ item.effectiveTo ? formatDate(item.effectiveTo) : 'Active' }}
                  </template>
                  
                  <template v-slot:item.isActive="{ }">
                    <v-chip
                      color="default"
                      size="small"
                      variant="tonal"
                    >
                      Completed
                    </v-chip>
                  </template>
                </v-data-table>
              </div>
            </div>
          </v-card-text>
        </v-card>

        <!-- Create/Edit Budget Dialog -->
        <v-dialog v-model="showDialog" max-width="600px" rounded="xl">
          <v-card rounded="xl">
            <v-card-title class="d-flex align-center">
              <v-icon :icon="editingBudget ? 'mdi-pencil' : 'mdi-plus'" class="mr-2" />
              {{ editingBudget ? 'Edit Budget' : 'Create Budget' }}
              <v-spacer></v-spacer>
              <v-btn
                icon
                variant="text"
                @click="closeDialog"
              >
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-card-title>
            
            <v-card-text>
              <v-form ref="formRef" v-model="formValid">
                <v-select
                  v-model="form.categoryId"
                  :items="availableCategoriesForBudget"
                  item-title="name"
                  item-value="id"
                  label="Category"
                  :rules="categoryRules"
                  variant="outlined"
                  rounded="xl"
                  class="mb-4"
                  :disabled="!!editingBudget"
                ></v-select>
                
                <v-text-field
                  v-model.number="form.amount"
                  label="Budget Amount"
                  type="number"
                  :rules="amountRules"
                  variant="outlined"
                  rounded="xl"
                  prepend-inner-icon="mdi-currency-usd"
                  min="0.01"
                  step="0.01"
                ></v-text-field>
              </v-form>
            </v-card-text>
            
            <v-card-actions class="px-6 pb-6">
              <v-spacer></v-spacer>
              <v-btn
                variant="text"
                @click="closeDialog"
                rounded="xl"
              >
                Cancel
              </v-btn>
              <v-btn
                color="primary"
                @click="saveBudget"
                :loading="saving"
                :disabled="!formValid"
                rounded="xl"
              >
                {{ editingBudget ? 'Update' : 'Create' }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-container>
    </v-main>

    <AppFooter />
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import { budgetService } from '@/lib/budgetService'
import { categoryService } from '@/lib/categoryService'
import type { BudgetDto, CreateBudgetDto, UpdateBudgetDto, BudgetStatusDto } from '@/types/budget'
import type { CategoryDto } from '@/types/category'

// Reactive state
const activeTab = ref('progress')
const isLoading = ref(false)
const saving = ref(false)
const showDialog = ref(false)
const editingBudget = ref<BudgetDto | null>(null)
const formValid = ref(false)

const budgetProgress = ref<BudgetStatusDto[]>([])
const activeBudgets = ref<BudgetDto[]>([])
const budgetHistory = ref<BudgetDto[]>([])
const expenseCategories = ref<CategoryDto[]>([])

// Computed: Filter out categories that already have active budgets when creating a new budget
const availableCategoriesForBudget = computed(() => {
  if (!editingBudget.value) {
    // When creating new budget, exclude categories with active budgets
    const activeCategoryIds = new Set(activeBudgets.value.map(budget => budget.categoryId))
    return expenseCategories.value.filter(category => !activeCategoryIds.has(category.id))
  }
  // When editing, show all categories (but the dropdown is disabled anyway)
  return expenseCategories.value
})

// Form data
const form = ref({
  categoryId: '',
  amount: 0
})

// Form validation rules
const categoryRules = [
  (v: string) => !!v || 'Category is required'
]

const amountRules = [
  (v: number) => !!v || 'Amount is required',
  (v: number) => v > 0 || 'Amount must be greater than zero',
  (v: number) => v <= 999999.99 || 'Amount must be less than $1,000,000'
]

// Table headers for history
const historyHeaders = [
  { title: 'Category', key: 'category', sortable: false },
  { title: 'Amount', key: 'amount', sortable: false },
  { title: 'Effective From', key: 'effectiveFrom', sortable: false },
  { title: 'Effective To', key: 'effectiveTo', sortable: false },
  { title: 'Status', key: 'isActive', sortable: false }
]

// Table headers for active budgets
const activeBudgetHeaders = [
  { title: 'Category', key: 'category', sortable: false },
  { title: 'Amount', key: 'amount', sortable: false },
  { title: 'Effective From', key: 'effectiveFrom', sortable: false },
  { title: 'Actions', key: 'actions', sortable: false, width: '100px' }
]

const toast = useToast()

// Methods
const loadData = async () => {
  isLoading.value = true
  try {
    await Promise.all([
      loadExpenseCategories(),
      loadBudgetProgress(),
      loadActiveBudgets(),
      loadBudgetHistory(),
      
    ])
  } catch (error: any) {
    console.error('Failed to load budget data:', error)
    toast.error('Failed to load budget data')
  } finally {
    isLoading.value = false
  }
}

const loadBudgetProgress = async () => {
  try {
    budgetProgress.value = await budgetService.getStatuses()
  } catch (error: any) {
    console.error('Failed to load budget progress:', error)
    toast.error('Failed to load budget progress')
  }
}

const loadActiveBudgets = async () => {
  try {
    activeBudgets.value = await budgetService.getActive()
  } catch (error: any) {
    console.error('Failed to load active budgets:', error)
    toast.error('Failed to load active budgets')
  }
}

const loadBudgetHistory = async () => {
  try {
    const allBudgets = await budgetService.getHistory()
    // Filter only completed budgets (isActive = false)
    budgetHistory.value = allBudgets.filter(budget => !budget.isActive)
  } catch (error: any) {
    console.error('Failed to load budget history:', error)
    toast.error('Failed to load budget history')
  }
}

const loadExpenseCategories = async () => {
  try {
    console.log('Loading expense categories...')
    const categories = await categoryService.getExpenseCategories()
    console.log('Loaded categories:', categories)
    expenseCategories.value = categories
    
    if (categories.length === 0) {
      toast.warning('No expense categories found. Please create some expense categories first.')
    }
  } catch (error: any) {
    console.error('Failed to load categories:', error)
    const errorMessage = error.response?.data?.message || error.response?.data?.error || 'Failed to load categories'
    toast.error(`Failed to load categories: ${errorMessage}`)
    expenseCategories.value = []
  }
}

const openCreateDialog = () => {
  editingBudget.value = null
  form.value = {
    categoryId: '',
    amount: 0
  }
  showDialog.value = true
}

const openEditDialogFromTable = (budget: BudgetDto) => {
  editingBudget.value = budget
  form.value = {
    categoryId: budget.categoryId,
    amount: budget.amount
  }
  showDialog.value = true
}

const openEditDialog = (budget: BudgetStatusDto) => {
  editingBudget.value = {
    id: budget.budgetId,
    userId: '', // Not needed for editing
    categoryId: budget.categoryId,
    amount: budget.allocatedAmount,
    effectiveFrom: budget.effectiveFrom,
    effectiveTo: budget.effectiveTo,
    isActive: budget.isActive,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    category: null
  }
  form.value = {
    categoryId: budget.categoryId,
    amount: budget.allocatedAmount
  }
  showDialog.value = true
}

const closeDialog = () => {
  showDialog.value = false
  editingBudget.value = null
  form.value = {
    categoryId: '',
    amount: 0
  }
}

const saveBudget = async () => {
  if (!formValid.value) return
  
  saving.value = true
  try {
    if (editingBudget.value) {
      const updateData: UpdateBudgetDto = {
        id: editingBudget.value.id,
        amount: form.value.amount
      }
      await budgetService.update(editingBudget.value.id, updateData)
      toast.success('Budget updated successfully')
    } else {
      const createData: CreateBudgetDto = {
        categoryId: form.value.categoryId,
        amount: form.value.amount
      }
      await budgetService.create(createData)
      toast.success('Budget created successfully')
    }
    
    await loadData()
    closeDialog()
  } catch (error: any) {
    console.error('Failed to save budget:', error)
    const errorMessage = error.response?.data?.error || 'Failed to save budget'
    toast.error(errorMessage)
  } finally {
    saving.value = false
  }
}

// Utility functions
const formatCurrency = (amount: number): string => {
  return amount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const getBudgetProgress = (budget: BudgetStatusDto): number => {
  return budget.percentageUsed
}

const getBudgetStatusColor = (budget: BudgetStatusDto): string => {
  return budget.statusColor
}

const getBudgetStatusIcon = (budget: BudgetStatusDto): string => {
  const progress = budget.percentageUsed
  if (progress >= 100) return 'mdi-alert-circle'
  if (progress >= 80) return 'mdi-alert'
  if (progress >= 40) return 'mdi-check-circle'
  return 'mdi-play-circle'
}

const getBudgetStatusIconColor = (): string => {
  return 'white'
}

const getBudgetStatusText = (budget: BudgetStatusDto): string => {
  return budget.status
}

// Lifecycle
onMounted(() => {
  loadData()
})
</script>

<style scoped>
.budget-card {
  transition: all 0.3s ease;
  cursor: pointer;
}

.budget-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(var(--v-theme-primary), 0.15);
}

.budget-history-table {
  border-radius: 12px;
}

.card-icon {
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.1), rgba(var(--v-theme-primary), 0.05));
}

.card-icon--primary {
  border: 2px solid rgba(var(--v-theme-primary), 0.2);
}
</style>
