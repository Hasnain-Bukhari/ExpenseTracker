<template>
  <div>
    <AppHeader />
    <AppNav />
    <v-main>
      <v-container fluid class="pa-6">
        <div class="accounts-page">
          <!-- Page Header -->
          <div class="page-header d-flex justify-space-between align-center mb-6">
            <div>
              <h2 class="text-h5 mb-1">Accounts</h2>
              <p class="text-body-2 text-medium-emphasis mb-0">Manage your financial accounts and track balances</p>
            </div>

            <div class="actions d-flex gap-3">
              <v-btn 
                color="primary" 
                variant="elevated" 
                @click="openCreateDialog"
                prepend-icon="mdi-plus"
              >
                New Account
              </v-btn>
            </div>
          </div>

          <!-- Loading State -->
          <div v-if="loading" class="d-flex justify-center py-8">
            <v-progress-circular indeterminate color="primary" size="64" />
          </div>

          <!-- Accounts Grid -->
          <v-row v-else-if="accounts.length > 0">
            <v-col 
              cols="12" 
              sm="6" 
              md="4" 
              lg="3" 
              v-for="account in accounts" 
              :key="account.id"
            >
              <v-card 
                class="account-card h-100"
                elevation="2"
                hover
              >
                <v-card-text class="pa-4">
                  <!-- Card Header -->
                  <div class="d-flex justify-space-between align-start mb-3">
                    <div class="flex-grow-1">
                      <div class="d-flex align-center mb-2">
                        <v-icon 
                          :icon="getAccountIcon(account)"
                          :color="getAccountColor(account)"
                          size="24"
                          class="mr-2"
                        />
                        <h3 class="text-subtitle-1 mb-0 font-weight-medium">
                          {{ account.name }}
                        </h3>
                      </div>
                      <div class="d-flex gap-2 mb-2">
                        <v-chip 
                          :color="getAccountTypeColor(account)"
                          size="small"
                          variant="tonal"
                        >
                          {{ getAccountTypeName(account.accountTypeId) }}
                        </v-chip>
                        <v-chip 
                          v-if="account.isSavings"
                          color="success"
                          size="small"
                          variant="tonal"
                        >
                          Savings
                        </v-chip>
                      </div>
                    </div>
                    
                    <!-- Actions Menu -->
                    <v-menu>
                      <template v-slot:activator="{ props }">
                        <v-btn
                          icon
                          variant="text"
                          size="small"
                          v-bind="props"
                          class="ml-2"
                        >
                          <v-icon icon="mdi-dots-vertical" />
                        </v-btn>
                      </template>
                      <v-list density="compact">
                        <v-list-item @click="editAccount(account)">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-pencil" size="small" />
                          </template>
                          <v-list-item-title>Edit</v-list-item-title>
                        </v-list-item>
                        <v-list-item @click="deleteAccount(account)" class="text-error">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-delete" size="small" color="error" />
                          </template>
                          <v-list-item-title>Delete</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </div>

                  <!-- Balance Display -->
                  <div class="balance-section mb-3">
                    <div class="text-h6 font-weight-bold" :class="getBalanceColor(account.openingBalance)">
                      {{ formatCurrency(account.openingBalance) }}
                    </div>
                    <div class="text-caption text-medium-emphasis">
                      Opening Balance
                    </div>
                  </div>

                  <!-- Card Footer -->
                  <v-divider class="my-3" />
                  <div class="d-flex justify-space-between align-center">
                    <div class="text-caption text-medium-emphasis">
                      Created {{ formatDate(account.createdAt) }}
                    </div>
                    <v-icon 
                      :icon="account.includeInNetworth ? 'mdi-chart-line' : 'mdi-chart-line-variant'"
                      size="16"
                      :color="account.includeInNetworth ? 'success' : 'medium-emphasis'"
                    />
                  </div>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>

          <!-- Empty State -->
          <div v-else class="empty-state text-center py-12">
            <v-icon 
              icon="mdi-bank-outline" 
              size="64" 
              color="medium-emphasis" 
              class="mb-4"
            />
            <h3 class="text-h6 mb-2">No Accounts</h3>
            <p class="text-body-2 text-medium-emphasis mb-4">
              Get started by creating your first account
            </p>
            <v-btn 
              color="primary" 
              variant="elevated" 
              @click="openCreateDialog"
              prepend-icon="mdi-plus"
            >
              Create Account
            </v-btn>
          </div>

          <!-- Create/Edit Dialog -->
          <v-dialog v-model="showDialog" persistent max-width="600">
            <v-card>
              <v-card-title class="text-h6 pa-6 pb-0">
                {{ editingAccount ? 'Edit Account' : 'New Account' }}
              </v-card-title>
              
              <v-card-text class="pa-6">
                <v-form ref="formRef" v-model="formValid">
                  <v-row>
                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model="form.name"
                        label="Account Name"
                        placeholder="e.g., Chase Checking, Savings Account"
                        :rules="nameRules"
                        variant="outlined"
                        prepend-inner-icon="mdi-bank"
                      />
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-select
                        v-model="form.accountTypeId"
                        :items="accountTypeOptions"
                        label="Account Type"
                        :rules="[v => !!v || 'Account type is required']"
                        variant="outlined"
                        prepend-inner-icon="mdi-tag"
                      />
                    </v-col>
                  </v-row>

                  <v-row>
                    <v-col cols="12" md="6">
                      <v-select
                        v-model="form.currencyId"
                        :items="currencyOptions"
                        label="Currency"
                        :rules="[v => !!v || 'Currency is required']"
                        variant="outlined"
                        prepend-inner-icon="mdi-currency-usd"
                      />
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model.number="form.openingBalance"
                        label="Opening Balance"
                        type="number"
                        step="0.01"
                        :rules="balanceRules"
                        variant="outlined"
                        prepend-inner-icon="mdi-cash"
                      />
                    </v-col>
                  </v-row>

                  <v-row>
                    <v-col cols="12" md="6">
                      <v-switch
                        v-model="form.isSavings"
                        label="This is a savings account"
                        color="success"
                        inset
                      />
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-switch
                        v-model="form.includeInNetworth"
                        label="Include in net worth calculation"
                        color="primary"
                        inset
                      />
                    </v-col>
                  </v-row>
                </v-form>
              </v-card-text>
              
              <v-card-actions class="pa-6 pt-0">
                <v-spacer />
                <v-btn 
                  variant="text" 
                  @click="closeDialog"
                  :disabled="saving"
                >
                  Cancel
                </v-btn>
                <v-btn 
                  color="primary" 
                  variant="elevated"
                  @click="saveAccount" 
                  :loading="saving"
                  :disabled="!formValid"
                >
                  {{ editingAccount ? 'Update' : 'Create' }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!-- Delete Confirmation Dialog -->
          <v-dialog v-model="showDeleteDialog" persistent max-width="400">
            <v-card>
              <v-card-title class="text-h6 pa-6 pb-0">
                Delete Account
              </v-card-title>
              
              <v-card-text class="pa-6">
                <div class="d-flex align-center mb-4">
                  <v-icon icon="mdi-alert-circle" color="warning" size="24" class="mr-3" />
                  <span class="text-body-1">
                    Are you sure you want to delete 
                    <strong>"{{ accountToDelete?.name }}"</strong>?
                  </span>
                </div>
                <p class="text-body-2 text-medium-emphasis">
                  This action cannot be undone. All transaction history for this account will be lost.
                </p>
              </v-card-text>
              
              <v-card-actions class="pa-6 pt-0">
                <v-spacer />
                <v-btn 
                  variant="text" 
                  @click="closeDeleteDialog"
                  :disabled="deleting"
                >
                  Cancel
                </v-btn>
                <v-btn 
                  color="error" 
                  variant="elevated"
                  @click="confirmDelete" 
                  :loading="deleting"
                >
                  Delete
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </div>
      </v-container>
    </v-main>

    <AppFooter />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive, computed } from 'vue'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import { accountService, accountTypeService, currencyService } from '@/services/apiService'
import type { AccountDto, CreateAccountDto, UpdateAccountDto } from '@/types/account'
import type { AccountTypeDto } from '@/types/accountType'
import type { CurrencyDto } from '@/types/currency'

// Reactive data
const accounts = ref<AccountDto[]>([])
const accountTypes = ref<AccountTypeDto[]>([])
const currencies = ref<CurrencyDto[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const showDialog = ref(false)
const showDeleteDialog = ref(false)
const formValid = ref(false)

const editingAccount = ref<AccountDto | null>(null)
const accountToDelete = ref<AccountDto | null>(null)

// Form data
const form = reactive<CreateAccountDto>({
  name: '',
  accountTypeId: '',
  currencyId: '',
  isSavings: false,
  openingBalance: 0,
  includeInNetworth: true
})

// Form validation rules
const nameRules = [
  (v: string) => !!v || 'Name is required',
  (v: string) => (v && v.length >= 2) || 'Name must be at least 2 characters',
  (v: string) => (v && v.length <= 50) || 'Name must be less than 50 characters'
]

const balanceRules = [
  (v: number) => v !== null && v !== undefined || 'Balance is required',
  (v: number) => !isNaN(v) || 'Balance must be a valid number'
]

// Computed properties
const formRef = ref()

const accountTypeOptions = computed(() => 
  accountTypes.value.map(type => ({
    title: type.name,
    value: type.id
  }))
)

const currencyOptions = computed(() => 
  currencies.value.map(currency => ({
    title: `${currency.code} - ${currency.name} (${currency.symbol})`,
    value: currency.id
  }))
)

// Methods
const loadAccounts = async () => {
  loading.value = true
  try {
    const data = await accountService.list()
    accounts.value = data || []
  } catch (error) {
    console.error('Failed to load accounts:', error)
    accounts.value = []
  } finally {
    loading.value = false
  }
}

const loadAccountTypes = async () => {
  try {
    const data = await accountTypeService.list()
    accountTypes.value = data || []
  } catch (error) {
    console.error('Failed to load account types:', error)
    accountTypes.value = []
  }
}

const loadCurrencies = async () => {
  try {
    const data = await currencyService.list()
    currencies.value = data || []
  } catch (error) {
    console.error('Failed to load currencies:', error)
    currencies.value = []
  }
}

const openCreateDialog = () => {
  editingAccount.value = null
  form.name = ''
  form.accountTypeId = ''
  form.currencyId = 'usd'
  form.isSavings = false
  form.openingBalance = 0
  form.includeInNetworth = true
  showDialog.value = true
}

const editAccount = (account: AccountDto) => {
  editingAccount.value = account
  form.name = account.name
  form.accountTypeId = account.accountTypeId
  form.currencyId = account.currencyId
  form.isSavings = account.isSavings
  form.openingBalance = account.openingBalance
  form.includeInNetworth = account.includeInNetworth
  showDialog.value = true
}

const closeDialog = () => {
  showDialog.value = false
  editingAccount.value = null
}

const saveAccount = async () => {
  if (!formValid.value) return
  
  saving.value = true
  try {
    if (editingAccount.value) {
      const updateData: UpdateAccountDto = {
        id: editingAccount.value.id,
        ...form
      }
      await accountService.update(editingAccount.value.id, updateData)
    } else {
      await accountService.create(form)
    }
    await loadAccounts()
    closeDialog()
  } catch (error) {
    console.error('Failed to save account:', error)
  } finally {
    saving.value = false
  }
}

const deleteAccount = (account: AccountDto) => {
  accountToDelete.value = account
  showDeleteDialog.value = true
}

const closeDeleteDialog = () => {
  showDeleteDialog.value = false
  accountToDelete.value = null
}

const confirmDelete = async () => {
  if (!accountToDelete.value) return
  
  deleting.value = true
  try {
    await accountService.delete(accountToDelete.value.id)
    await loadAccounts()
    closeDeleteDialog()
  } catch (error) {
    console.error('Failed to delete account:', error)
  } finally {
    deleting.value = false
  }
}

// Helper methods
const getAccountIcon = (account: AccountDto) => {
  const accountType = accountTypes.value.find(t => t.id === account.accountTypeId)
  if (accountType?.isCard) return 'mdi-credit-card'
  if (account.isSavings) return 'mdi-piggy-bank'
  return 'mdi-bank'
}

const getAccountColor = (account: AccountDto) => {
  const accountType = accountTypes.value.find(t => t.id === account.accountTypeId)
  if (accountType?.isCard) return 'primary'
  if (account.isSavings) return 'success'
  return 'info'
}

const getAccountTypeColor = (account: AccountDto) => {
  const accountType = accountTypes.value.find(t => t.id === account.accountTypeId)
  if (accountType?.isCard) return 'primary'
  return 'info'
}

const getAccountTypeName = (accountTypeId: string) => {
  const accountType = accountTypes.value.find(t => t.id === accountTypeId)
  return accountType?.name || 'Unknown'
}

const getBalanceColor = (balance: number) => {
  if (balance > 0) return 'text-success'
  if (balance < 0) return 'text-error'
  return 'text-medium-emphasis'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadAccounts(),
    loadAccountTypes(),
    loadCurrencies()
  ])
})
</script>

<style scoped>
.accounts-page {
  min-height: 100vh;
}

.account-card {
  transition: all 0.3s ease;
  border-radius: 12px;
}

.account-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1) !important;
}

.empty-state {
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.05) 0%, rgba(var(--v-theme-secondary), 0.05) 100%);
  border-radius: 16px;
  border: 2px dashed rgba(var(--v-theme-primary), 0.2);
}

.page-header {
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.1) 0%, rgba(var(--v-theme-secondary), 0.1) 100%);
  padding: 24px;
  border-radius: 16px;
  margin-bottom: 24px;
}

.balance-section {
  background: rgba(var(--v-theme-surface-variant), 0.3);
  padding: 12px;
  border-radius: 8px;
  text-align: center;
}

.actions {
  flex-shrink: 0;
}

@media (max-width: 600px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start !important;
    gap: 16px;
  }
  
  .actions {
    width: 100%;
    justify-content: flex-end;
  }
}
</style>