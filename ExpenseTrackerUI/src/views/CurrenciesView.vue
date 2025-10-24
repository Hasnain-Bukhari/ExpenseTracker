<template>
  <div>
    <AppHeader />
    <AppNav />
    <v-main>
      <v-container fluid class="pa-6">
        <div class="currencies-page">
          <!-- Page Header -->
          <div class="page-header d-flex justify-space-between align-center mb-6">
            <div>
              <h2 class="text-h5 mb-1">Currencies</h2>
              <p class="text-body-2 text-medium-emphasis mb-0">Manage currencies and exchange rates</p>
            </div>

            <div class="actions d-flex gap-3">
              <v-btn 
                color="primary" 
                variant="elevated" 
                @click="openCreateDialog"
                prepend-icon="mdi-plus"
              >
                New Currency
              </v-btn>
            </div>
          </div>

          <!-- Loading State -->
          <div v-if="loading" class="d-flex justify-center py-8">
            <v-progress-circular indeterminate color="primary" size="64" />
          </div>

          <!-- Currencies Grid -->
          <v-row v-else-if="currencies.length > 0">
            <v-col 
              cols="12" 
              sm="6" 
              md="4" 
              lg="3" 
              v-for="currency in currencies" 
              :key="currency.id"
            >
              <v-card 
                class="currency-card h-100"
                elevation="2"
                hover
              >
                <v-card-text class="pa-4">
                  <!-- Card Header -->
                  <div class="d-flex justify-space-between align-start mb-3">
                    <div class="flex-grow-1">
                      <div class="d-flex align-center mb-2">
                        <v-icon 
                          icon="mdi-currency-usd"
                          color="primary"
                          size="24"
                          class="mr-2"
                        />
                        <h3 class="text-subtitle-1 mb-0 font-weight-medium">
                          {{ currency.code }}
                        </h3>
                      </div>
                      <div class="d-flex gap-2 mb-2">
                        <v-chip 
                          color="primary"
                          size="small"
                          variant="tonal"
                        >
                          {{ currency.symbol }}
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
                        <v-list-item @click="editCurrency(currency)">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-pencil" size="small" />
                          </template>
                          <v-list-item-title>Edit</v-list-item-title>
                        </v-list-item>
                        <v-list-item @click="deleteCurrency(currency)" class="text-error">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-delete" size="small" color="error" />
                          </template>
                          <v-list-item-title>Delete</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </div>

                  <!-- Currency Info -->
                  <div class="currency-info mb-3">
                    <div class="text-h6 font-weight-bold text-primary mb-1">
                      {{ currency.name }}
                    </div>
                    <div class="text-caption text-medium-emphasis">
                      Currency Code: {{ currency.code }}
                    </div>
                  </div>

                  <!-- Card Footer -->
                  <v-divider class="my-3" />
                  <div class="d-flex justify-space-between align-center">
                    <div class="text-caption text-medium-emphasis">
                      Created {{ formatDate(currency.createdAt) }}
                    </div>
                    <v-icon 
                      icon="mdi-check-circle"
                      size="16"
                      color="success"
                    />
                  </div>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>

          <!-- Empty State -->
          <div v-else class="empty-state text-center py-12">
            <v-icon 
              icon="mdi-currency-usd" 
              size="64" 
              color="medium-emphasis" 
              class="mb-4"
            />
            <h3 class="text-h6 mb-2">No Currencies</h3>
            <p class="text-body-2 text-medium-emphasis mb-4">
              Get started by adding your first currency
            </p>
            <v-btn 
              color="primary" 
              variant="elevated" 
              @click="openCreateDialog"
              prepend-icon="mdi-plus"
            >
              Add Currency
            </v-btn>
          </div>

          <!-- Create/Edit Dialog -->
          <v-dialog v-model="showDialog" persistent max-width="600">
            <v-card>
              <v-card-title class="text-h6 pa-6 pb-0">
                {{ editingCurrency ? 'Edit Currency' : 'New Currency' }}
              </v-card-title>
              
              <v-card-text class="pa-6">
                <v-form ref="formRef" v-model="formValid">
                  <v-row>
                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model="form.code"
                        label="Currency Code"
                        placeholder="e.g., USD, EUR, GBP"
                        :rules="codeRules"
                        variant="outlined"
                        prepend-inner-icon="mdi-tag"
                        hint="3-letter ISO currency code"
                        persistent-hint
                      />
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model="form.symbol"
                        label="Currency Symbol"
                        placeholder="e.g., $, €, £"
                        :rules="symbolRules"
                        variant="outlined"
                        prepend-inner-icon="mdi-currency-usd"
                        hint="Currency symbol"
                        persistent-hint
                      />
                    </v-col>
                  </v-row>

                  <v-row>
                    <v-col cols="12">
                      <v-text-field
                        v-model="form.name"
                        label="Currency Name"
                        placeholder="e.g., US Dollar, Euro, British Pound"
                        :rules="nameRules"
                        variant="outlined"
                        prepend-inner-icon="mdi-text"
                        hint="Full currency name"
                        persistent-hint
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
                  @click="saveCurrency" 
                  :loading="saving"
                  :disabled="!formValid"
                >
                  {{ editingCurrency ? 'Update' : 'Create' }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!-- Delete Confirmation Dialog -->
          <v-dialog v-model="showDeleteDialog" persistent max-width="400">
            <v-card>
              <v-card-title class="text-h6 pa-6 pb-0">
                Delete Currency
              </v-card-title>
              
              <v-card-text class="pa-6">
                <div class="d-flex align-center mb-4">
                  <v-icon icon="mdi-alert-circle" color="warning" size="24" class="mr-3" />
                  <span class="text-body-1">
                    Are you sure you want to delete 
                    <strong>"{{ currencyToDelete?.name }} ({{ currencyToDelete?.code }})"</strong>?
                  </span>
                </div>
                <p class="text-body-2 text-medium-emphasis">
                  This action cannot be undone. All accounts using this currency will be affected.
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
import { currencyService } from '@/services/apiService'
import type { CurrencyDto, CreateCurrencyDto, UpdateCurrencyDto } from '@/types/currency'

// Reactive data
const currencies = ref<CurrencyDto[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const showDialog = ref(false)
const showDeleteDialog = ref(false)
const formValid = ref(false)

const editingCurrency = ref<CurrencyDto | null>(null)
const currencyToDelete = ref<CurrencyDto | null>(null)

// Form data
const form = reactive<CreateCurrencyDto>({
  code: '',
  name: '',
  symbol: ''
})

// Form validation rules
const codeRules = [
  (v: string) => !!v || 'Currency code is required',
  (v: string) => (v && v.length === 3) || 'Currency code must be exactly 3 characters',
  (v: string) => (v && /^[A-Z]{3}$/.test(v)) || 'Currency code must be uppercase letters only'
]

const nameRules = [
  (v: string) => !!v || 'Currency name is required',
  (v: string) => (v && v.length >= 2) || 'Currency name must be at least 2 characters',
  (v: string) => (v && v.length <= 50) || 'Currency name must be less than 50 characters'
]

const symbolRules = [
  (v: string) => !!v || 'Currency symbol is required',
  (v: string) => (v && v.length >= 1) || 'Currency symbol is required',
  (v: string) => (v && v.length <= 5) || 'Currency symbol must be less than 5 characters'
]

// Computed properties
const formRef = ref()

// Methods
const loadCurrencies = async () => {
  loading.value = true
  try {
    const data = await currencyService.list()
    currencies.value = data || []
  } catch (error) {
    console.error('Failed to load currencies:', error)
    currencies.value = []
  } finally {
    loading.value = false
  }
}

const openCreateDialog = () => {
  editingCurrency.value = null
  form.code = ''
  form.name = ''
  form.symbol = ''
  showDialog.value = true
}

const editCurrency = (currency: CurrencyDto) => {
  editingCurrency.value = currency
  form.code = currency.code
  form.name = currency.name
  form.symbol = currency.symbol
  showDialog.value = true
}

const closeDialog = () => {
  showDialog.value = false
  editingCurrency.value = null
}

const saveCurrency = async () => {
  if (!formValid.value) return
  
  saving.value = true
  try {
    if (editingCurrency.value) {
      const updateData: UpdateCurrencyDto = {
        id: editingCurrency.value.id,
        ...form
      }
      await currencyService.update(editingCurrency.value.id, updateData)
    } else {
      await currencyService.create(form)
    }
    await loadCurrencies()
    closeDialog()
  } catch (error) {
    console.error('Failed to save currency:', error)
  } finally {
    saving.value = false
  }
}

const deleteCurrency = (currency: CurrencyDto) => {
  currencyToDelete.value = currency
  showDeleteDialog.value = true
}

const closeDeleteDialog = () => {
  showDeleteDialog.value = false
  currencyToDelete.value = null
}

const confirmDelete = async () => {
  if (!currencyToDelete.value) return
  
  deleting.value = true
  try {
    await currencyService.delete(currencyToDelete.value.id)
    await loadCurrencies()
    closeDeleteDialog()
  } catch (error) {
    console.error('Failed to delete currency:', error)
  } finally {
    deleting.value = false
  }
}

// Helper methods
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

// Lifecycle
onMounted(async () => {
  await loadCurrencies()
})
</script>

<style scoped>
.currencies-page {
  min-height: 100vh;
}

.currency-card {
  transition: all 0.3s ease;
  border-radius: 12px;
}

.currency-card:hover {
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

.currency-info {
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
