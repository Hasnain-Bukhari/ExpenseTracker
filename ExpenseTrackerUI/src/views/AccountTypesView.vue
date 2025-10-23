<template>
  <div>
    <AppHeader />
    <AppNav />
    <v-main>
      <v-container fluid class="pa-6">
        <div class="account-types-page">
          <!-- Page Header -->
          <div class="page-header d-flex justify-space-between align-center mb-6">
            <div>
              <h2 class="text-h5 mb-1">Account Types</h2>
              <p class="text-body-2 text-medium-emphasis mb-0">Manage different types of accounts and cards</p>
            </div>

            <div class="actions d-flex gap-3">
              <v-btn 
                color="primary" 
                variant="elevated" 
                @click="openCreateDialog"
                prepend-icon="mdi-plus"
              >
                New Account Type
              </v-btn>
            </div>
          </div>

          <!-- Loading State -->
          <div v-if="loading" class="d-flex justify-center py-8">
            <v-progress-circular indeterminate color="primary" size="64" />
          </div>

          <!-- Account Types Grid -->
          <v-row v-else-if="accountTypes.length > 0">
            <v-col 
              cols="12" 
              sm="6" 
              md="4" 
              lg="3" 
              v-for="accountType in accountTypes" 
              :key="accountType.id"
            >
              <v-card 
                class="account-type-card h-100"
                :class="{ 'card-type': accountType.isCard }"
                elevation="2"
                hover
              >
                <v-card-text class="pa-4">
                  <!-- Card Header -->
                  <div class="d-flex justify-space-between align-start mb-3">
                    <div class="flex-grow-1">
                      <div class="d-flex align-center mb-2">
                        <v-icon 
                          :icon="accountType.isCard ? 'mdi-credit-card' : 'mdi-bank'"
                          :color="accountType.isCard ? 'primary' : 'success'"
                          size="24"
                          class="mr-2"
                        />
                        <h3 class="text-subtitle-1 mb-0 font-weight-medium">
                          {{ accountType.name }}
                        </h3>
                      </div>
                      <v-chip 
                        :color="accountType.isCard ? 'primary' : 'success'"
                        size="small"
                        variant="tonal"
                        class="mb-2"
                      >
                        {{ accountType.isCard ? 'Card' : 'Account' }}
                      </v-chip>
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
                        <v-list-item @click="editAccountType(accountType)">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-pencil" size="small" />
                          </template>
                          <v-list-item-title>Edit</v-list-item-title>
                        </v-list-item>
                        <v-list-item @click="deleteAccountType(accountType)" class="text-error">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-delete" size="small" color="error" />
                          </template>
                          <v-list-item-title>Delete</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </div>

                  <!-- Card Footer -->
                  <v-divider class="my-3" />
                  <div class="d-flex justify-space-between align-center">
                    <div class="text-caption text-medium-emphasis">
                      Created {{ formatDate(accountType.createdAt) }}
                    </div>
                    <v-icon 
                      :icon="accountType.isCard ? 'mdi-credit-card-outline' : 'mdi-bank-outline'"
                      size="16"
                      class="text-medium-emphasis"
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
            <h3 class="text-h6 mb-2">No Account Types</h3>
            <p class="text-body-2 text-medium-emphasis mb-4">
              Get started by creating your first account type
            </p>
            <v-btn 
              color="primary" 
              variant="elevated" 
              @click="openCreateDialog"
              prepend-icon="mdi-plus"
            >
              Create Account Type
            </v-btn>
          </div>

          <!-- Create/Edit Dialog -->
          <v-dialog v-model="showDialog" persistent max-width="500">
            <v-card>
              <v-card-title class="text-h6 pa-6 pb-0">
                {{ editingAccountType ? 'Edit Account Type' : 'New Account Type' }}
              </v-card-title>
              
              <v-card-text class="pa-6">
                <v-form ref="formRef" v-model="formValid">
                  <v-text-field
                    v-model="form.name"
                    label="Account Type Name"
                    placeholder="e.g., Checking, Savings, Credit Card"
                    :rules="nameRules"
                    variant="outlined"
                    class="mb-4"
                    prepend-inner-icon="mdi-tag"
                  />
                  
                  <v-switch
                    v-model="form.isCard"
                    label="This is a card type"
                    color="primary"
                    inset
                    class="mt-2"
                  />
                  
                  <div v-if="form.isCard" class="mt-2 pa-3 bg-primary-lighten-5 rounded">
                    <div class="d-flex align-center">
                      <v-icon icon="mdi-information" color="primary" class="mr-2" />
                      <span class="text-body-2 text-primary">
                        Card types are typically used for credit cards, debit cards, and similar payment methods.
                      </span>
                    </div>
                  </div>
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
                  @click="saveAccountType" 
                  :loading="saving"
                  :disabled="!formValid"
                >
                  {{ editingAccountType ? 'Update' : 'Create' }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!-- Delete Confirmation Dialog -->
          <v-dialog v-model="showDeleteDialog" persistent max-width="400">
            <v-card>
              <v-card-title class="text-h6 pa-6 pb-0">
                Delete Account Type
              </v-card-title>
              
              <v-card-text class="pa-6">
                <div class="d-flex align-center mb-4">
                  <v-icon icon="mdi-alert-circle" color="warning" size="24" class="mr-3" />
                  <span class="text-body-1">
                    Are you sure you want to delete 
                    <strong>"{{ accountTypeToDelete?.name }}"</strong>?
                  </span>
                </div>
                <p class="text-body-2 text-medium-emphasis">
                  This action cannot be undone. Any accounts using this type may be affected.
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
import { accountTypeApi } from '@/lib/api'
import type { AccountTypeDto, CreateAccountTypeDto, UpdateAccountTypeDto } from '@/types/accountType'

// Reactive data
const accountTypes = ref<AccountTypeDto[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const showDialog = ref(false)
const showDeleteDialog = ref(false)
const formValid = ref(false)

const editingAccountType = ref<AccountTypeDto | null>(null)
const accountTypeToDelete = ref<AccountTypeDto | null>(null)

// Form data
const form = reactive<CreateAccountTypeDto>({
  name: '',
  isCard: false
})

// Form validation rules
const nameRules = [
  (v: string) => !!v || 'Name is required',
  (v: string) => (v && v.length >= 2) || 'Name must be at least 2 characters',
  (v: string) => (v && v.length <= 50) || 'Name must be less than 50 characters'
]

// Computed properties
const formRef = ref()

// Methods
const loadAccountTypes = async () => {
  loading.value = true
  try {
    const data = await accountTypeApi.list()
    accountTypes.value = data || []
  } catch (error) {
    console.error('Failed to load account types:', error)
    accountTypes.value = []
  } finally {
    loading.value = false
  }
}

const openCreateDialog = () => {
  editingAccountType.value = null
  form.name = ''
  form.isCard = false
  showDialog.value = true
}

const editAccountType = (accountType: AccountTypeDto) => {
  editingAccountType.value = accountType
  form.name = accountType.name
  form.isCard = accountType.isCard
  showDialog.value = true
}

const closeDialog = () => {
  showDialog.value = false
  editingAccountType.value = null
  form.name = ''
  form.isCard = false
}

const saveAccountType = async () => {
  if (!formValid.value) return
  
  saving.value = true
  try {
    if (editingAccountType.value) {
      await accountTypeApi.update(editingAccountType.value.id, form)
    } else {
      await accountTypeApi.create(form)
    }
    await loadAccountTypes()
    closeDialog()
  } catch (error) {
    console.error('Failed to save account type:', error)
  } finally {
    saving.value = false
  }
}

const deleteAccountType = (accountType: AccountTypeDto) => {
  accountTypeToDelete.value = accountType
  showDeleteDialog.value = true
}

const closeDeleteDialog = () => {
  showDeleteDialog.value = false
  accountTypeToDelete.value = null
}

const confirmDelete = async () => {
  if (!accountTypeToDelete.value) return
  
  deleting.value = true
  try {
    await accountTypeApi.delete(accountTypeToDelete.value.id)
    await loadAccountTypes()
    closeDeleteDialog()
  } catch (error) {
    console.error('Failed to delete account type:', error)
  } finally {
    deleting.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

// Lifecycle
onMounted(() => {
  loadAccountTypes()
})
</script>

<style scoped>
.account-types-page {
  min-height: 100vh;
}

.account-type-card {
  transition: all 0.3s ease;
  border-radius: 12px;
}

.account-type-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1) !important;
}

.account-type-card.card-type {
  border-left: 4px solid rgb(var(--v-theme-primary));
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
