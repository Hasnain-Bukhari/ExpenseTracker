<template>
  <div class="transactions-page">
    <AppHeader />
    <AppNav />
    <v-main>
      <v-container fluid class="py-8">
        <v-row>
          <v-col cols="12" md="10" offset-md="1">
            <!-- Filters Card -->
            <v-card rounded="xl" elevation="2" class="mb-6">
              <v-card-title class="d-flex align-center">
                <v-icon icon="mdi-filter" class="mr-2"></v-icon>
                Filters
                <v-spacer></v-spacer>
                <v-btn
                  variant="text"
                  @click="clearFilters"
                  class="text-capitalize"
                >
                  Clear Filters
                </v-btn>
              </v-card-title>
              <v-card-text>
                <v-row>
                  <v-col cols="12" md="3">
                    <v-select
                      v-model="filters.accountId"
                      :items="accountOptions"
                      label="Account"
                      clearable
                      variant="outlined"
                      rounded="xl"
                    ></v-select>
                  </v-col>
                  <v-col cols="12" md="3">
                    <v-select
                      v-model="filters.categoryId"
                      :items="categoryOptions"
                      label="Category"
                      clearable
                      variant="outlined"
                      rounded="xl"
                    ></v-select>
                  </v-col>
                  <v-col cols="12" md="3">
                    <v-text-field
                      v-model="filters.startDate"
                      label="Start Date"
                      type="date"
                      variant="outlined"
                      rounded="xl"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" md="3">
                    <v-text-field
                      v-model="filters.endDate"
                      label="End Date"
                      type="date"
                      variant="outlined"
                      rounded="xl"
                    ></v-text-field>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <!-- Transactions Card -->
            <v-card rounded="xl" elevation="2" class="mb-8">
              <v-card-title class="d-flex align-center pe-2">
                <v-icon icon="mdi-swap-horizontal" class="mr-2"></v-icon>
                Transactions
                <v-spacer></v-spacer>
                <v-btn
                  color="primary"
                  prepend-icon="mdi-plus"
                  @click="openCreateDialog"
                  rounded="xl"
                  class="text-capitalize"
                >
                  Add Transaction
                </v-btn>
              </v-card-title>
              <v-card-text>
                <!-- Loading State -->
                <div v-if="loading" class="text-center py-8">
                  <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                  <p class="mt-4 text-body-1">Loading transactions...</p>
                </div>

                <!-- Empty State -->
                <div v-else-if="transactions.length === 0" class="empty-state">
                  <v-icon icon="mdi-swap-horizontal" class="empty-state-icon"></v-icon>
                  <h3 class="empty-state-title">No Transactions Found</h3>
                  <p class="empty-state-subtitle">Start by adding your first transaction</p>
                  <v-btn
                    color="primary"
                    prepend-icon="mdi-plus"
                    @click="openCreateDialog"
                    rounded="xl"
                    class="text-capitalize mt-4"
                  >
                    Add Transaction
                  </v-btn>
                </div>

                <!-- Transactions Table -->
                <v-data-table
                  v-else
                  :headers="headers"
                  :items="transactions"
                  :loading="loading"
                  class="elevation-0"
                  :items-per-page="pagination.pageSize"
                  :page="pagination.page"
                  :server-items-length="pagination.total"
                  @update:page="onPageChange"
                  @update:items-per-page="onPageSizeChange"
                >
                  <template v-slot:item.amount="{ item }">
                    <span :class="getAmountClass(item.category?.categoryType?.name)">
                      {{ formatAmount(item.amount, item.category?.categoryType?.name) }}
                    </span>
                  </template>

                  <template v-slot:item.transactionDate="{ item }">
                    {{ formatDate(item.transactionDate) }}
                  </template>

                  <template v-slot:item['category.categoryType.name']="{ item }">
                    <v-chip
                      :color="item.category?.categoryType?.color || 'primary'"
                      size="small"
                      variant="tonal"
                    >
                      {{ item.category?.categoryType?.name }}
                    </v-chip>
                  </template>

                  <template v-slot:item.actions="{ item }">
                    <v-menu>
                      <template v-slot:activator="{ props }">
                        <v-btn
                          icon="mdi-dots-vertical"
                          variant="text"
                          size="small"
                          v-bind="props"
                        ></v-btn>
                      </template>
                      <v-list>
                        <v-list-item @click="editTransaction(item)">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-pencil"></v-icon>
                          </template>
                          <v-list-item-title>Edit</v-list-item-title>
                        </v-list-item>
                        <v-list-item @click="confirmDelete(item)" class="text-error">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-delete" color="error"></v-icon>
                          </template>
                          <v-list-item-title>Delete</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </template>
                </v-data-table>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
    <AppFooter />

    <!-- Create/Edit Transaction Dialog -->
    <v-dialog v-model="showDialog" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon 
            :icon="editingTransaction ? 'mdi-pencil' : 'mdi-plus'"
            class="mr-2"
          ></v-icon>
          {{ editingTransaction ? 'Edit Transaction' : 'Create Transaction' }}
        </v-card-title>
        <v-card-text>
          <v-form ref="formRef" v-model="formValid">
            <v-row>
              <v-col cols="12" md="6">
                <v-select
                  v-model="form.accountId"
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
                  v-model="form.categoryId"
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
                  v-model="form.subCategoryId"
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
                  v-model="form.amount"
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
                  v-model="form.transactionDate"
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
              v-model="form.description"
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
            @click="closeDialog"
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
            {{ editingTransaction ? 'Update' : 'Create' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="showDeleteDialog" max-width="400px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-delete" color="error" class="mr-2"></v-icon>
          Delete Transaction
        </v-card-title>
        <v-card-text>
          <p>Are you sure you want to delete this transaction?</p>
          <p class="text-body-2 text-medium-emphasis">
            <strong>{{ transactionToDelete?.description }}</strong>
          </p>
        </v-card-text>
        <v-card-actions class="px-6 pb-4">
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="showDeleteDialog = false"
            rounded="xl"
            class="text-capitalize"
          >
            Cancel
          </v-btn>
          <v-btn
            color="error"
            variant="flat"
            @click="deleteTransaction"
            :loading="deleting"
            rounded="xl"
            class="text-capitalize"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive, computed, watch } from 'vue'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import { transactionApi, accountApi, categoryApi } from '@/lib/api'
import type { TransactionDto, CreateTransactionDto, UpdateTransactionDto, PagedResult } from '@/types'
import type { AccountDto } from '@/types/account'
import type { CategoryDto } from '@/types/category'

// Reactive data
const transactions = ref<TransactionDto[]>([])
const accounts = ref<AccountDto[]>([])
const categories = ref<CategoryDto[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const showDialog = ref(false)
const showDeleteDialog = ref(false)
const formValid = ref(false)

const editingTransaction = ref<TransactionDto | null>(null)
const transactionToDelete = ref<TransactionDto | null>(null)

const form = reactive<CreateTransactionDto>({
  accountId: '',
  categoryId: '',
  subCategoryId: null,
  description: '',
  amount: 0,
  transactionDate: new Date().toISOString().split('T')[0]
})

const filters = reactive({
  accountId: '',
  categoryId: '',
  startDate: '',
  endDate: ''
})

const pagination = reactive({
  page: 1,
  pageSize: 25,
  total: 0
})

// Table headers
const headers = [
  { title: 'Date', key: 'transactionDate', sortable: true },
  { title: 'Description', key: 'description', sortable: true },
  { title: 'Amount', key: 'amount', sortable: true },
  { title: 'Account', key: 'account.name', sortable: true },
  { title: 'Category', key: 'category.name', sortable: true },
  { title: 'Subcategory', key: 'subCategory.name', sortable: true },
  { title: 'Type', key: 'category.categoryType.name', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

// Computed properties
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
  if (!form.categoryId) return []
  const category = categories.value.find(c => c.id === form.categoryId)
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
const loadTransactions = async () => {
  loading.value = true
  try {
    const filterParams = {
      accountId: filters.accountId || undefined,
      categoryId: filters.categoryId || undefined,
      startDate: filters.startDate || undefined,
      endDate: filters.endDate || undefined,
      page: pagination.page,
      pageSize: pagination.pageSize
    }
    
    const result: PagedResult<TransactionDto> = await transactionApi.list(filterParams)
    transactions.value = result.items || []
    pagination.total = result.total
  } catch (error) {
    console.error('Failed to load transactions:', error)
    transactions.value = []
  } finally {
    loading.value = false
  }
}

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

const openCreateDialog = () => {
  editingTransaction.value = null
  form.accountId = ''
  form.categoryId = ''
  form.subCategoryId = ''
  form.description = ''
  form.amount = 0
  form.transactionDate = new Date().toISOString().split('T')[0]
  showDialog.value = true
}

const editTransaction = (transaction: TransactionDto) => {
  // Find the full transaction details
  const fullTransaction = transactions.value.find(t => t.id === transaction.id)
  if (!fullTransaction) return

  editingTransaction.value = {
    id: fullTransaction.id,
    userId: '',
    accountId: '',
    categoryId: '',
    subCategoryId: null,
    description: fullTransaction.description,
    amount: fullTransaction.amount,
    transactionDate: fullTransaction.transactionDate,
    createdAt: '',
    updatedAt: '',
    account: {} as AccountDto,
    category: {} as CategoryDto,
    subCategory: null
  }

  form.accountId = ''
  form.categoryId = ''
  form.subCategoryId = null
  form.description = fullTransaction.description
  form.amount = fullTransaction.amount
  form.transactionDate = fullTransaction.transactionDate
  showDialog.value = true
}

const closeDialog = () => {
  showDialog.value = false
}

const saveTransaction = async () => {
  if (!formValid.value) return

  saving.value = true
  try {
    if (editingTransaction.value) {
      await transactionApi.update(editingTransaction.value.id, {
        id: editingTransaction.value.id,
        ...form
      })
    } else {
      await transactionApi.create(form)
    }
    showDialog.value = false
    await loadTransactions()
  } catch (error) {
    console.error('Failed to save transaction:', error)
    // TODO: Show error notification
  } finally {
    saving.value = false
  }
}

const confirmDelete = (transaction: TransactionDto) => {
  transactionToDelete.value = transaction
  showDeleteDialog.value = true
}

const deleteTransaction = async () => {
  if (!transactionToDelete.value) return

  deleting.value = true
  try {
    await transactionApi.delete(transactionToDelete.value.id)
    showDeleteDialog.value = false
    await loadTransactions()
  } catch (error) {
    console.error('Failed to delete transaction:', error)
    // TODO: Show error notification
  } finally {
    deleting.value = false
  }
}

const clearFilters = () => {
  filters.accountId = ''
  filters.categoryId = ''
  filters.startDate = ''
  filters.endDate = ''
  pagination.page = 1
}

const onPageChange = (page: number) => {
  pagination.page = page
}

const onPageSizeChange = (pageSize: number) => {
  pagination.pageSize = pageSize
  pagination.page = 1
}

const onCategoryChange = () => {
  form.subCategoryId = null
}

const formatAmount = (amount: number, categoryType: string) => {
  const sign = categoryType === 'Expense' ? '-' : '+'
  return `${sign}$${amount.toFixed(2)}`
}

const getAmountClass = (categoryType: string) => {
  return categoryType === 'Expense' ? 'text-error' : 'text-success'
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

// Watchers
watch(filters, () => {
  pagination.page = 1
  loadTransactions()
}, { deep: true })

watch(pagination, () => {
  loadTransactions()
}, { deep: true })

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadAccounts(),
    loadCategories(),
    loadTransactions()
  ])
})
</script>

<style scoped>
.transactions-page {
  min-height: 100vh;
  background-color: #f5f7fa;
}

.v-card {
  background-color: #ffffff;
  border: 1px solid #e0e0e0;
}

.empty-state {
  text-align: center;
  padding: 40px 20px;
  color: #757575;
}

.empty-state-icon {
  font-size: 80px;
  color: #bdbdbd;
}

.empty-state-title {
  font-size: 1.5rem;
  font-weight: bold;
  margin-top: 10px;
}

.empty-state-subtitle {
  font-size: 1rem;
  margin-top: 5px;
}

.text-success {
  color: #4caf50 !important;
}

.text-error {
  color: #f44336 !important;
}
</style>
