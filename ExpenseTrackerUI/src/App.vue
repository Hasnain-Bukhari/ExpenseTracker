<template>
  <v-app>
    <router-view />
    
    <!-- Global Quick Add Dialogs -->
    <!-- Add Transaction Dialog -->
    <v-dialog v-model="showTransactionDialog" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-plus" class="mr-2"></v-icon>
          Add Transaction
        </v-card-title>
        <v-card-text>
          <v-form ref="transactionFormRef" v-model="transactionFormValid">
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
                  label="Subcategory (Optional)"
                  :rules="subCategoryRules"
                  variant="outlined"
                  rounded="xl"
                  class="mb-3"
                  clearable
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
            :loading="savingTransaction"
            :disabled="!transactionFormValid"
            rounded="xl"
            class="text-capitalize"
          >
            Add Transaction
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Add Goal Dialog -->
    <v-dialog v-model="showGoalDialog" max-width="600">
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
    <v-dialog v-model="showBudgetDialog" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-chart-line" class="mr-2"></v-icon>
          Create Budget
        </v-card-title>
        
        <v-card-text>
          <v-form ref="budgetFormRef" v-model="budgetFormValid">
            <v-select
              v-model="budgetForm.categoryId"
              :items="availableCategoriesForBudget"
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
  </v-app>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useToast } from 'vue-toastification'
import { useDialog } from '@/composables/useDialog'
import { transactionApi, accountApi, categoryApi } from '@/lib/api'
import { goalService } from '@/lib/goalService'
import { budgetService } from '@/lib/budgetService'
import { categoryService } from '@/lib/categoryService'
import type { CreateTransactionDto } from '@/types/transaction'
import type { AccountDto, CategoryDto } from '@/types'
import { GoalStatus, GoalPriority } from '@/types/goal'
import type { CreateGoalDto } from '@/types/goal'
import type { CreateBudgetDto } from '@/types/budget'

const toast = useToast()
const {
  showTransactionDialog,
  showGoalDialog,
  showBudgetDialog,
  closeTransactionDialog: closeTransactionDialogBase,
  closeGoalDialog: closeGoalDialogBase,
  closeBudgetDialog: closeBudgetDialogBase
} = useDialog()

// Form state
const transactionFormValid = ref(false)
const goalFormValid = ref(false)
const budgetFormValid = ref(false)
const savingTransaction = ref(false)
const savingGoal = ref(false)
const savingBudget = ref(false)

// Data
const accounts = ref<AccountDto[]>([])
const categories = ref<CategoryDto[]>([])
const savingsCategories = ref<CategoryDto[]>([])
const expenseCategories = ref<CategoryDto[]>([])
const activeBudgets = ref<any[]>([])

// Transaction form
const transactionForm = reactive<CreateTransactionDto>({
  accountId: '',
  categoryId: '',
  subCategoryId: '',
  description: '',
  amount: 0,
  transactionDate: new Date().toISOString().split('T')[0]
})

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

// Form options
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

// Filter out categories that already have active budgets
const availableCategoriesForBudget = computed(() => {
  const activeCategoryIds = new Set(activeBudgets.value.map((budget: any) => budget.categoryId))
  return expenseCategories.value.filter(category => !activeCategoryIds.has(category.id))
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

// Form rules
const accountRules = [(v: string) => !!v || 'Account is required']
const categoryRules = [(v: string) => !!v || 'Category is required']
const subCategoryRules: Array<(v: string | null | undefined) => boolean | string> = [] // Subcategory is optional
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
    // Use categoryService.list() which returns CategoryDto[] with subcategories
    const data = await categoryService.list()
    categories.value = data || []
    console.log('Loaded categories:', categories.value.length, 'categories')
    if (categories.value.length > 0) {
      console.log('First category subcategories:', categories.value[0]?.subCategories?.length || 0)
    }
  } catch (error) {
    console.error('Failed to load categories:', error)
    categories.value = []
  }
}

const loadSavingsCategories = async () => {
  try {
    const allCategories = await categoryService.list()
    const allSavingsCategories = allCategories.filter((cat: CategoryDto) => cat.categoryType === 'TargetedSavingsGoal')
    
    // Get active goals to filter out categories that already have active goals
    const activeGoalsList = await goalService.getActive()
    const activeCategoryIds = new Set(activeGoalsList.map(goal => goal.categoryId))
    
    // Filter out categories that already have active goals
    savingsCategories.value = allSavingsCategories.filter(cat => !activeCategoryIds.has(cat.id))
  } catch (error) {
    console.error('Failed to load savings categories:', error)
    savingsCategories.value = []
  }
}

const loadExpenseCategories = async () => {
  try {
    expenseCategories.value = await categoryService.getExpenseCategories()
  } catch (error) {
    console.error('Failed to load expense categories:', error)
    expenseCategories.value = []
  }
}

const loadActiveBudgets = async () => {
  try {
    activeBudgets.value = await budgetService.getActive()
  } catch (error) {
    console.error('Failed to load active budgets:', error)
    activeBudgets.value = []
  }
}

// Transaction methods
const onCategoryChange = () => {
  transactionForm.subCategoryId = ''
}

const closeTransactionDialog = () => {
  closeTransactionDialogBase()
  // Reset form
  transactionForm.accountId = ''
  transactionForm.categoryId = ''
  transactionForm.subCategoryId = ''
  transactionForm.description = ''
  transactionForm.amount = 0
  transactionForm.transactionDate = new Date().toISOString().split('T')[0]
}

const saveTransaction = async () => {
  if (!transactionFormValid.value) return

  savingTransaction.value = true
  try {
    // Prepare form data - convert empty string to null for optional subcategory
    const formData = {
      ...transactionForm,
      subCategoryId: transactionForm.subCategoryId || null
    }
    
    await transactionApi.create(formData)
    toast.success('Transaction created successfully')
    closeTransactionDialog()
  } catch (error: any) {
    console.error('Failed to save transaction:', error)
    const errorMessage = error.response?.data?.error || error.response?.data?.message || 'Failed to save transaction'
    toast.error(errorMessage)
  } finally {
    savingTransaction.value = false
  }
}

// Goal methods
const closeGoalDialog = () => {
  closeGoalDialogBase()
  // Reset form
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

const saveGoal = async () => {
  if (!goalFormValid.value) return
  
  savingGoal.value = true
  try {
    await goalService.create(goalForm)
    toast.success('Goal created successfully')
    closeGoalDialog()
  } catch (error: any) {
    console.error('Failed to save goal:', error)
    const errorMessage = error.response?.data?.error || error.response?.data?.message || 'Failed to save goal'
    toast.error(errorMessage)
  } finally {
    savingGoal.value = false
  }
}

// Budget methods
const closeBudgetDialog = () => {
  closeBudgetDialogBase()
  // Reset form
  budgetForm.categoryId = ''
  budgetForm.amount = 0
}

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
    // Reload active budgets to update the category filter
    await loadActiveBudgets()
  } catch (error: any) {
    console.error('Failed to save budget:', error)
    const errorMessage = error.response?.data?.error || error.response?.data?.message || 'Failed to save budget'
    toast.error(errorMessage)
  } finally {
    savingBudget.value = false
  }
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadAccounts(),
    loadCategories(),
    loadSavingsCategories(),
    loadExpenseCategories(),
    loadActiveBudgets()
  ])
})

// Watch for dialog opening and ensure data is loaded
watch(showTransactionDialog, async (isOpen) => {
  if (isOpen) {
    // Reload data when dialog opens to ensure it's fresh
    await Promise.all([
      loadAccounts(),
      loadCategories()
    ])
  }
})

watch(showGoalDialog, async (isOpen) => {
  if (isOpen) {
    await loadSavingsCategories()
  }
})

watch(showBudgetDialog, async (isOpen) => {
  if (isOpen) {
    await Promise.all([
      loadExpenseCategories(),
      loadActiveBudgets()
    ])
  }
})
</script>
