<template>
  <div class="categories-page">
    <AppHeader />
    <AppNav />
    <v-main>
      <v-container fluid class="py-8">
        <v-row>
          <v-col cols="12" md="10" offset-md="1">
            <v-card rounded="xl" elevation="2" class="mb-8">
              <v-card-title class="d-flex align-center pe-2">
                <v-icon icon="mdi-tag-multiple" class="mr-2"></v-icon> Categories
                <v-spacer></v-spacer>
                <v-btn
                  color="primary"
                  prepend-icon="mdi-plus"
                  @click="openCreateCategory"
                  rounded="xl"
                  class="text-capitalize"
                >
                  Add Category
                </v-btn>
              </v-card-title>
              <v-card-text>
                <!-- Loading State -->
                <div v-if="loading" class="text-center py-8">
                  <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                  <p class="text-body-1 mt-4 text-medium-emphasis">Loading categories...</p>
                </div>

                <!-- Empty State -->
                <div v-else-if="!categories.length" class="empty-state">
                  <v-icon icon="mdi-tag-outline" class="empty-state-icon"></v-icon>
                  <h3 class="empty-state-title">No Categories Yet</h3>
                  <p class="empty-state-subtitle">Create your first category to organize your expenses and income</p>
                  <v-btn
                    color="primary"
                    prepend-icon="mdi-plus"
                    @click="openCreateCategory"
                    rounded="xl"
                    class="mt-4"
                  >
                    Create Category
                  </v-btn>
                </div>

                <!-- Categories Grid -->
                <v-row v-else>
                  <v-col 
                    cols="12" 
                    sm="6" 
                    md="4" 
                    v-for="cat in categories" 
                    :key="cat.id"
                  >
                    <v-card 
                      class="category-card h-100"
                      rounded="xl"
                      elevation="1"
                      hover
                    >
                      <v-card-title class="d-flex align-center justify-space-between pb-2">
                        <div class="d-flex align-center">
                          <v-avatar 
                            :color="getCategoryTypeColor(cat.categoryType)" 
                            size="40"
                            class="mr-3"
                          >
                            <v-icon 
                              icon="mdi-tag"
                              color="white"
                            ></v-icon>
                          </v-avatar>
                          <div>
                            <h3 class="text-h6 mb-0">{{ cat.name }}</h3>
                            <v-chip 
                              :color="getCategoryTypeColor(cat.categoryType)"
                              size="x-small"
                              variant="tonal"
                              class="mt-1"
                            >
                              {{ getCategoryTypeName(cat.categoryType) }}
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
                            <v-list-item @click="editCategory(cat)">
                              <template v-slot:prepend>
                                <v-icon icon="mdi-pencil"></v-icon>
                              </template>
                              <v-list-item-title>Edit</v-list-item-title>
                            </v-list-item>
                            <v-list-item @click="confirmDeleteCategory(cat)" class="text-error">
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
                        <p v-if="cat.description" class="text-body-2 text-medium-emphasis mb-3">
                          {{ cat.description }}
                        </p>

                        <!-- Subcategories -->
                        <div v-if="cat.subCategories && cat.subCategories.length" class="mb-3">
                          <div class="d-flex align-center justify-space-between mb-2">
                            <h4 class="text-subtitle-2 mb-0">Subcategories</h4>
                            <v-chip size="x-small" variant="tonal" color="primary">
                              {{ cat.subCategories.length }}
                            </v-chip>
                          </div>
                          <div class="subcategories-container">
                            <v-chip
                              v-for="sub in cat.subCategories.slice(0, 3)"
                              :key="sub.id"
                              size="small"
                              variant="outlined"
                              class="ma-1"
                            >
                              {{ sub.name }}
                            </v-chip>
                            <v-chip
                              v-if="cat.subCategories.length > 3"
                              size="small"
                              variant="tonal"
                              color="primary"
                              class="ma-1"
                            >
                              +{{ cat.subCategories.length - 3 }} more
                            </v-chip>
                          </div>
                        </div>

                        <!-- No subcategories message -->
                        <div v-else class="mb-3">
                          <p class="text-caption text-medium-emphasis">No subcategories yet</p>
                        </div>
                      </v-card-text>

                      <v-card-actions class="pt-0">
                        <v-btn
                          size="small"
                          variant="text"
                          prepend-icon="mdi-plus"
                          @click="openCreateSub(cat)"
                          class="text-capitalize"
                        >
                          Add Subcategory
                        </v-btn>
                        <v-spacer></v-spacer>
                        <v-btn
                          size="small"
                          variant="text"
                          prepend-icon="mdi-eye"
                          @click="viewSubcategories(cat)"
                          class="text-capitalize"
                        >
                          View All
                        </v-btn>
                      </v-card-actions>
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

    <!-- Create/Edit Category Dialog -->
    <v-dialog v-model="showCategoryDialog" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon 
            :icon="editingCategory ? 'mdi-pencil' : 'mdi-plus'"
            class="mr-2"
          ></v-icon>
          {{ editingCategory ? 'Edit Category' : 'Create Category' }}
        </v-card-title>
        <v-card-text>
          <v-form ref="categoryFormRef" v-model="categoryFormValid">
            <v-text-field
              v-model="categoryForm.name"
              label="Category Name"
              :rules="nameRules"
              variant="outlined"
              rounded="xl"
              class="mb-3"
            ></v-text-field>
            
            <v-select
              v-model="categoryForm.categoryType"
              :items="categoryTypeOptions"
              label="Category Type"
              :rules="categoryTypeRules"
              variant="outlined"
              rounded="xl"
              class="mb-3"
            ></v-select>
            
            <v-textarea
              v-model="categoryForm.description"
              label="Description (Optional)"
              variant="outlined"
              rounded="xl"
              rows="3"
            ></v-textarea>
          </v-form>
        </v-card-text>
        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            @click="closeCategoryDialog"
            rounded="xl"
          >
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            @click="saveCategory"
            :loading="saving"
            :disabled="!categoryFormValid"
            rounded="xl"
          >
            {{ editingCategory ? 'Update' : 'Create' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Create/Edit Subcategory Dialog -->
    <v-dialog v-model="showSubDialog" max-width="600px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon 
            :icon="editingSub ? 'mdi-pencil' : 'mdi-plus'"
            class="mr-2"
          ></v-icon>
          {{ editingSub ? 'Edit Subcategory' : 'Create Subcategory' }}
          <v-chip v-if="currentParentCat" size="small" variant="tonal" class="ml-2">
            {{ currentParentCat.name }}
          </v-chip>
        </v-card-title>
        <v-card-text>
          <v-form ref="subFormRef" v-model="subFormValid">
            <v-text-field
              v-model="subForm.name"
              label="Subcategory Name"
              :rules="nameRules"
              variant="outlined"
              rounded="xl"
              class="mb-3"
            ></v-text-field>
            
            <v-textarea
              v-model="subForm.description"
              label="Description (Optional)"
              variant="outlined"
              rounded="xl"
              rows="3"
            ></v-textarea>
          </v-form>
        </v-card-text>
        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            @click="closeSubDialog"
            rounded="xl"
          >
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            @click="saveSub"
            :loading="saving"
            :disabled="!subFormValid"
            rounded="xl"
          >
            {{ editingSub ? 'Update' : 'Create' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Subcategories View Dialog -->
    <v-dialog v-model="showSubcategoriesDialog" max-width="800px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-tag-multiple" class="mr-2"></v-icon>
          Subcategories
          <v-chip v-if="selectedCategory" size="small" variant="tonal" class="ml-2">
            {{ selectedCategory.name }}
          </v-chip>
        </v-card-title>
        <v-card-text>
          <div v-if="selectedCategory && selectedCategory.subCategories && selectedCategory.subCategories.length">
            <v-row>
              <v-col 
                cols="12" 
                sm="6" 
                v-for="sub in selectedCategory.subCategories" 
                :key="sub.id"
              >
                <v-card variant="outlined" rounded="lg" class="pa-3">
                  <div class="d-flex align-center justify-space-between">
                    <div>
                      <h4 class="text-subtitle-1 mb-1">{{ sub.name }}</h4>
                      <p v-if="sub.description" class="text-caption text-medium-emphasis mb-0">
                        {{ sub.description }}
                      </p>
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
                        <v-list-item @click="editSubCategory(selectedCategory, sub)">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-pencil"></v-icon>
                          </template>
                          <v-list-item-title>Edit</v-list-item-title>
                        </v-list-item>
                        <v-list-item @click="confirmDeleteSubCategory(selectedCategory, sub)" class="text-error">
                          <template v-slot:prepend>
                            <v-icon icon="mdi-delete" color="error"></v-icon>
                          </template>
                          <v-list-item-title>Delete</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </div>
                </v-card>
              </v-col>
            </v-row>
          </div>
          <div v-else class="text-center py-8">
            <v-icon icon="mdi-tag-outline" size="64" class="text-medium-emphasis"></v-icon>
            <p class="text-body-1 mt-4 text-medium-emphasis">No subcategories found</p>
          </div>
        </v-card-text>
        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            @click="selectedCategory && openCreateSub(selectedCategory)"
            rounded="xl"
          >
            Add Subcategory
          </v-btn>
          <v-btn
            variant="text"
            @click="showSubcategoriesDialog = false"
            rounded="xl"
          >
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="showDeleteDialog" max-width="400px" rounded="xl">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center text-error">
          <v-icon icon="mdi-delete" class="mr-2"></v-icon>
          Confirm Delete
        </v-card-title>
        <v-card-text>
          <p class="text-body-1">
            Are you sure you want to delete 
            <strong>{{ itemToDelete?.name }}</strong>?
          </p>
          <p v-if="itemToDelete && 'type' in itemToDelete && itemToDelete.type === 'category'" class="text-caption text-medium-emphasis">
            This will also delete all associated subcategories.
          </p>
        </v-card-text>
        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            @click="showDeleteDialog = false"
            rounded="xl"
          >
            Cancel
          </v-btn>
          <v-btn
            color="error"
            @click="deleteItem"
            :loading="deleting"
            rounded="xl"
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
import { useToast } from 'vue-toastification'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import api, { getAccessToken } from '@/lib/api'
import type { CategoryDto, CreateCategoryDto, CreateSubCategoryDto, SubCategoryDto } from '@/types/category'
import { CategoryType } from '@/types/category'

// Reactive data
const toast = useToast()
const categories = ref<CategoryDto[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const showCategoryDialog = ref(false)
const showSubDialog = ref(false)
const showSubcategoriesDialog = ref(false)
const showDeleteDialog = ref(false)
const categoryFormValid = ref(false)
const subFormValid = ref(false)

const editingCategory = ref<CategoryDto | null>(null)
const editingSub = ref<SubCategoryDto | null>(null)
const selectedCategory = ref<CategoryDto | null>(null)
const itemToDelete = ref<CategoryDto | SubCategoryDto | null>(null)

const categoryForm = reactive<CreateCategoryDto>({ name: '', categoryType: CategoryType.Expense, description: '' })
const subForm = reactive<CreateSubCategoryDto>({ name: '', description: '' })
const currentParentCat = ref<CategoryDto | null>(null)

// Form rules
const nameRules = [
  (v: string) => !!v || 'Name is required',
  (v: string) => (v && v.length >= 2) || 'Name must be at least 2 characters'
]

const categoryTypeRules = [
  (v: CategoryType) => !!v || 'Category type is required'
]

// Computed properties
const categoryTypeOptions = computed(() => [
  { title: 'Income', value: CategoryType.Income },
  { title: 'Expense', value: CategoryType.Expense },
  { title: 'Targeted Savings Goal', value: CategoryType.TargetedSavingsGoal }
])

// Helper functions
const getCategoryTypeColor = (categoryType: CategoryType): string => {
  switch (categoryType) {
    case CategoryType.Income: return 'success'
    case CategoryType.Expense: return 'error'
    case CategoryType.TargetedSavingsGoal: return 'info'
    default: return 'default'
  }
}

const getCategoryTypeName = (categoryType: CategoryType): string => {
  switch (categoryType) {
    case CategoryType.Income: return 'Income'
    case CategoryType.Expense: return 'Expense'
    case CategoryType.TargetedSavingsGoal: return 'Targeted Savings Goal'
    default: return 'Unknown'
  }
}

const authHeader = () => {
  const t = getAccessToken()
  return t ? { Authorization: `Bearer ${t}` } : {}
}

const load = async () => {
  loading.value = true
  try {
    const r = await api.get('/categories', { headers: authHeader() })
    console.log('Loaded categories', r.data)
    categories.value = r.data || []
    console.log('Loaded categories', r.data.categories)
  } catch (e) {
    // optionally show error; keep console for now
    console.error('Failed to load categories', e)
    categories.value = []
  } finally {
    loading.value = false
  }
}

const openCreateCategory = () => {
  editingCategory.value = null
  categoryForm.name = ''
  categoryForm.categoryType = CategoryType.Expense
  categoryForm.description = ''
  showCategoryDialog.value = true
}

const editCategory = (cat: CategoryDto) => {
  editingCategory.value = cat
  categoryForm.name = cat.name
  categoryForm.categoryType = cat.categoryType
  categoryForm.description = cat.description || ''
  showCategoryDialog.value = true
}

const closeCategoryDialog = () => showCategoryDialog.value = false

const saveCategory = async () => {
  saving.value = true
  try {
    const headers = authHeader()
    if (editingCategory.value) {
      await api.put(`/categories/${editingCategory.value.id}`, { 
        id: editingCategory.value.id,
        name: categoryForm.name, 
        categoryType: categoryForm.categoryType,
        description: categoryForm.description 
      }, { headers })
    } else {
      await api.post('/categories', { 
        name: categoryForm.name, 
        categoryType: categoryForm.categoryType, 
        description: categoryForm.description 
      }, { headers })
    }
    await load()
    closeCategoryDialog()
  } catch (e) {
    console.error('Failed to save category', e)
  } finally {
    saving.value = false
  }
}

// Removed old deleteCategory method - now using confirmDeleteCategory

const openCreateSub = (cat: CategoryDto) => {
  currentParentCat.value = cat
  editingSub.value = null
  subForm.name = ''
  subForm.description = ''
  showSubDialog.value = true
}

const editSubCategory = (cat: CategoryDto, sub: SubCategoryDto) => {
  currentParentCat.value = cat
  editingSub.value = sub
  subForm.name = sub.name
  subForm.description = sub.description || ''
  showSubDialog.value = true
}

const closeSubDialog = () => showSubDialog.value = false

const saveSub = async () => {
  if (!currentParentCat.value) return
  saving.value = true
  try {
    const headers = authHeader()
    if (editingSub.value) {
      await api.put(`/subcategories/${editingSub.value.id}`, { name: subForm.name, description: subForm.description }, { headers })
    } else {
      await api.post(`/categories/${currentParentCat.value.id}/subcategories`, { name: subForm.name, description: subForm.description }, { headers })
    }
    await load()
    closeSubDialog()
  } catch (e) {
    console.error('Failed to save subcategory', e)
  } finally {
    saving.value = false
  }
}

// Removed old deleteSubCategory method - now using confirmDeleteSubCategory

// New methods for enhanced UI
const confirmDeleteCategory = (cat: CategoryDto) => {
  itemToDelete.value = cat
  showDeleteDialog.value = true
}

const confirmDeleteSubCategory = (_cat: CategoryDto, sub: SubCategoryDto) => {
  itemToDelete.value = sub
  showDeleteDialog.value = true
}

const deleteItem = async () => {
  if (!itemToDelete.value) return
  
  deleting.value = true
  try {
    const headers = authHeader()
    if ('categoryType' in itemToDelete.value) {
      // It's a category
      await api.delete(`/categories/${itemToDelete.value.id}`, { headers })
      toast.success('Category deleted successfully')
    } else {
      // It's a subcategory
      await api.delete(`/subcategories/${itemToDelete.value.id}`, { headers })
      toast.success('Subcategory deleted successfully')
    }
    await load()
    showDeleteDialog.value = false
  } catch (error: any) {
    console.error('Failed to delete item', error)
    
    // Check if it's a foreign key constraint error
    if (error.response?.status === 400 || error.response?.status === 409) {
      const errorMessage = error.response?.data?.message || error.response?.data?.error || 'Cannot delete this item because it is being used by transactions'
      toast.error(errorMessage)
    } else if (error.response?.status === 404) {
      toast.error('Item not found')
    } else {
      toast.error('Failed to delete item. Please try again.')
    }
  } finally {
    deleting.value = false
  }
}

const viewSubcategories = (cat: CategoryDto) => {
  selectedCategory.value = cat
  showSubcategoriesDialog.value = true
}

onMounted(async () => {
  await load()
})
</script>

<style scoped>
.categories-page {
  min-height: 100vh;
  background-color: rgb(var(--v-theme-background));
}

.v-card {
  background-color: rgb(var(--v-theme-surface));
}

.category-card {
  transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

.category-card:hover {
  transform: translateY(-2px);
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

.subcategories-container {
  max-height: 120px;
  overflow-y: auto;
}

/* Custom scrollbar for subcategories */
.subcategories-container::-webkit-scrollbar {
  width: 4px;
}

.subcategories-container::-webkit-scrollbar-track {
  background: rgb(var(--v-theme-surface-variant));
  border-radius: 2px;
}

.subcategories-container::-webkit-scrollbar-thumb {
  background: rgb(var(--v-theme-outline));
  border-radius: 2px;
}

.subcategories-container::-webkit-scrollbar-thumb:hover {
  background: rgba(var(--v-theme-outline), 0.8);
}
</style>

