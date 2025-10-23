<template>
  <div class="category-types-page">
    <AppHeader title="Category Types" />
    <AppNav />
    <v-main>
      <v-container fluid class="py-8">
        <v-row>
          <v-col cols="12" md="10" offset-md="1">
            <v-card rounded="xl" elevation="2" class="mb-8">
              <v-card-title class="d-flex align-center pe-2">
                <v-icon icon="mdi-tag-multiple" class="mr-2"></v-icon> Category Types
                <v-spacer></v-spacer>
                <v-btn
                  color="primary"
                  prepend-icon="mdi-plus"
                  @click="openCreateDialog"
                  rounded="xl"
                  class="text-capitalize"
                >
                  Add Category Type
                </v-btn>
              </v-card-title>
              <v-card-text>
                <!-- Loading State -->
                <div v-if="loading" class="text-center py-8">
                  <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                  <p class="mt-4 text-body-1">Loading category types...</p>
                </div>

                <!-- Empty State -->
                <div v-else-if="categoryTypes.length === 0" class="empty-state">
                  <v-icon icon="mdi-tag-multiple-outline" class="empty-state-icon"></v-icon>
                  <h3 class="empty-state-title">No Category Types</h3>
                  <p class="empty-state-subtitle">Create your first category type to get started</p>
                </div>

                <!-- Category Types Grid -->
                <v-row v-else>
                  <v-col
                    v-for="categoryType in categoryTypes"
                    :key="categoryType.id"
                    cols="12"
                    sm="6"
                    md="4"
                    lg="3"
                  >
                    <v-card 
                      class="category-type-card h-100"
                      rounded="xl"
                      elevation="1"
                      hover
                    >
                      <v-card-title class="d-flex align-center justify-space-between pb-2">
                        <div class="d-flex align-center">
                          <v-avatar 
                            :color="categoryType.color || 'primary'" 
                            size="40"
                            class="mr-3"
                          >
                            <v-icon 
                              icon="mdi-tag"
                              color="white"
                            ></v-icon>
                          </v-avatar>
                          <div>
                            <h3 class="text-h6 mb-0">{{ categoryType.name }}</h3>
                            <v-chip 
                              :color="categoryType.isActive ? 'success' : 'error'"
                              size="x-small"
                              variant="tonal"
                              class="mt-1"
                            >
                              {{ categoryType.isActive ? 'Active' : 'Inactive' }}
                            </v-chip>
                          </div>
                        </div>
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
                            <v-list-item @click="openEditDialog(categoryType)">
                              <template v-slot:prepend>
                                <v-icon icon="mdi-pencil"></v-icon>
                              </template>
                              <v-list-item-title>Edit</v-list-item-title>
                            </v-list-item>
                            <v-list-item @click="confirmDelete(categoryType)" class="text-error">
                              <template v-slot:prepend>
                                <v-icon icon="mdi-delete" color="error"></v-icon>
                              </template>
                              <v-list-item-title>Delete</v-list-item-title>
                            </v-list-item>
                          </v-list>
                        </v-menu>
                      </v-card-title>

                      <v-card-text class="pt-0">
                        <!-- Description -->
                        <p v-if="categoryType.description" class="text-body-2 text-medium-emphasis mb-3">
                          {{ categoryType.description }}
                        </p>

                        <!-- Color Preview -->
                        <div v-if="categoryType.color" class="mb-3">
                          <div class="d-flex align-center">
                            <span class="text-caption text-medium-emphasis mr-2">Color:</span>
                            <v-chip
                              :color="categoryType.color"
                              size="small"
                              variant="flat"
                            >
                              {{ categoryType.color }}
                            </v-chip>
                          </div>
                        </div>

                        <!-- Created Date -->
                        <div class="text-caption text-medium-emphasis">
                          Created: {{ formatDate(categoryType.createdAt) }}
                        </div>
                      </v-card-text>
                    </v-card>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
    <AppFooter />

    <!-- Create/Edit Category Type Dialog -->
    <v-dialog v-model="showDialog" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon :icon="editingCategoryType ? 'mdi-pencil' : 'mdi-plus'" class="mr-2"></v-icon>
          {{ dialogTitle }}
        </v-card-title>
        
        <v-form v-model="formValid" @submit.prevent="saveCategoryType">
          <v-card-text>
            <v-text-field
              v-model="form.name"
              label="Name"
              :rules="nameRules"
              required
              variant="outlined"
              rounded="xl"
              class="mb-4"
            ></v-text-field>

            <v-textarea
              v-model="form.description"
              label="Description"
              variant="outlined"
              rounded="xl"
              rows="3"
              class="mb-4"
            ></v-textarea>

            <v-text-field
              v-model="form.color"
              label="Color (Hex Code)"
              placeholder="#f44336"
              variant="outlined"
              rounded="xl"
              class="mb-4"
            ></v-text-field>

            <v-switch
              v-model="form.isActive"
              label="Active"
              color="success"
              inset
            ></v-switch>
          </v-card-text>

          <v-card-actions class="px-6 pb-6">
            <v-spacer></v-spacer>
            <v-btn
              variant="text"
              @click="showDialog = false"
              class="text-capitalize"
            >
              Cancel
            </v-btn>
            <v-btn
              color="primary"
              type="submit"
              :loading="saving"
              :disabled="!formValid"
              rounded="xl"
              class="text-capitalize"
            >
              {{ saveButtonText }}
            </v-btn>
          </v-card-actions>
        </v-form>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="showDeleteDialog" max-width="400px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center text-error">
          <v-icon icon="mdi-delete" class="mr-2"></v-icon>
          Delete Category Type
        </v-card-title>
        
        <v-card-text>
          <p>Are you sure you want to delete <strong>{{ categoryTypeToDelete?.name }}</strong>?</p>
          <p class="text-caption text-medium-emphasis mt-2">
            This action cannot be undone. Categories using this type will need to be updated.
          </p>
        </v-card-text>

        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            @click="showDeleteDialog = false"
            class="text-capitalize"
          >
            Cancel
          </v-btn>
          <v-btn
            color="error"
            @click="deleteCategoryType"
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
import { ref, onMounted, reactive, computed } from 'vue'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import { categoryTypeApi } from '@/lib/api'
import type { CategoryTypeDto, CreateCategoryTypeDto, UpdateCategoryTypeDto } from '@/types/categoryType'

// Reactive data
const categoryTypes = ref<CategoryTypeDto[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const showDialog = ref(false)
const showDeleteDialog = ref(false)
const formValid = ref(false)

const editingCategoryType = ref<CategoryTypeDto | null>(null)
const categoryTypeToDelete = ref<CategoryTypeDto | null>(null)

const form = reactive<CreateCategoryTypeDto | UpdateCategoryTypeDto>({
  name: '',
  description: '',
  color: '',
  isActive: true
})

// Form rules
const nameRules = [
  (v: string) => !!v || 'Name is required',
  (v: string) => (v && v.length >= 2) || 'Name must be at least 2 characters'
]

// Computed properties
const dialogTitle = computed(() => (editingCategoryType.value ? 'Edit Category Type' : 'Create Category Type'))
const saveButtonText = computed(() => (editingCategoryType.value ? 'Update' : 'Create'))

// Methods
const loadCategoryTypes = async () => {
  loading.value = true
  try {
    const data = await categoryTypeApi.list()
    categoryTypes.value = data || []
  } catch (error) {
    console.error('Failed to load category types:', error)
    categoryTypes.value = []
  } finally {
    loading.value = false
  }
}

const openCreateDialog = () => {
  editingCategoryType.value = null
  form.name = ''
  form.description = ''
  form.color = ''
  form.isActive = true
  showDialog.value = true
}

const openEditDialog = (categoryType: CategoryTypeDto) => {
  editingCategoryType.value = { ...categoryType }
  form.name = categoryType.name
  form.description = categoryType.description || ''
  form.color = categoryType.color || ''
  form.isActive = categoryType.isActive
  showDialog.value = true
}

const saveCategoryType = async () => {
  if (!formValid.value) return

  saving.value = true
  try {
    if (editingCategoryType.value) {
      await categoryTypeApi.update(editingCategoryType.value.id, { id: editingCategoryType.value.id, ...form })
    } else {
      await categoryTypeApi.create(form as CreateCategoryTypeDto)
    }
    showDialog.value = false
    await loadCategoryTypes()
  } catch (error) {
    console.error('Failed to save category type:', error)
    // TODO: Show error notification
  } finally {
    saving.value = false
  }
}

const confirmDelete = (categoryType: CategoryTypeDto) => {
  categoryTypeToDelete.value = categoryType
  showDeleteDialog.value = true
}

const deleteCategoryType = async () => {
  if (!categoryTypeToDelete.value) return

  deleting.value = true
  try {
    await categoryTypeApi.delete(categoryTypeToDelete.value.id)
    showDeleteDialog.value = false
    await loadCategoryTypes()
  } catch (error) {
    console.error('Failed to delete category type:', error)
    // TODO: Show error notification
  } finally {
    deleting.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

// Lifecycle
onMounted(loadCategoryTypes)
</script>

<style scoped>
.category-types-page {
  min-height: 100vh;
  background-color: #f5f7fa;
}

.v-card {
  background-color: #ffffff;
  border: 1px solid #e0e0e0;
}

.category-type-card {
  transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

.category-type-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
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
</style>
