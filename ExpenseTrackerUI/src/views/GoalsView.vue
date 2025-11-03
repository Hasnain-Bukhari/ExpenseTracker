<template>
  <v-app>
    <AppHeader />
    <AppNav />
    
    <v-main>
      <v-container fluid class="pa-6">
        <!-- Header -->
        <div class="d-flex align-center justify-space-between mb-6">
          <div>
            <h1 class="text-h3 font-weight-bold text-primary mb-2">
              <v-icon icon="mdi-target" class="mr-3" />
              Goals
            </h1>
            <p class="text-h6 text-medium-emphasis">
              Set and track your financial goals to stay motivated.
            </p>
          </div>
          <v-btn
            color="primary"
            size="large"
            prepend-icon="mdi-plus"
            @click="openCreateDialog"
          >
            New Goal
          </v-btn>
        </div>

        <!-- Tabs -->
        <v-card>
          <v-tabs
            v-model="activeTab"
            color="primary"
            align-tabs="start"
            class="v-tabs--grow"
          >
            <v-tab value="progress">
              <v-icon icon="mdi-chart-line" class="mr-2" />
              Goal Progress
            </v-tab>
            <v-tab value="active">
              <v-icon icon="mdi-play-circle" class="mr-2" />
              Active Goals
            </v-tab>
            <v-tab value="completed">
              <v-icon icon="mdi-check-circle" class="mr-2" />
              Completed Goals
            </v-tab>
          </v-tabs>

          <v-card-text>
            <v-window v-model="activeTab">
              <!-- Goal Progress Tab -->
              <v-window-item value="progress">
                <div v-if="isLoading" class="text-center py-8">
                  <v-progress-circular indeterminate color="primary" size="64" />
                  <p class="mt-4 text-medium-emphasis">Loading goal progress...</p>
                </div>
                <div v-else-if="goalProgress.length === 0" class="text-center py-8">
                  <v-icon icon="mdi-target" size="64" color="grey-lighten-1" class="mb-4" />
                  <h3 class="text-h5 mb-2">No Active Goals</h3>
                  <p class="text-medium-emphasis mb-4">Create your first goal to start tracking your progress.</p>
                  <v-btn color="primary" @click="openCreateDialog">
                    Create Goal
                  </v-btn>
                </div>
                <div v-else class="goal-progress-container">
                  <v-row dense>
                    <v-col
                      v-for="goal in goalProgress"
                      :key="goal.goalId"
                      cols="12"
                      md="6"
                      class="mb-4"
                    >
                      <v-card 
                        class="goal-progress-card h-100" 
                        elevation="3"
                        rounded="xl"
                        variant="outlined"
                      >
                        <!-- Card Header with gradient background -->
                        <v-card-title 
                          class="goal-card-header pa-4 pb-2"
                          :style="{ background: `linear-gradient(135deg, rgba(var(--v-theme-${goal.priorityColor}), 0.1) 0%, rgba(var(--v-theme-${goal.priorityColor}), 0.05) 100%)` }"
                        >
                          <div class="d-flex align-center justify-space-between w-100">
                            <div class="d-flex align-center flex-grow-1 min-width-0">
                              <v-avatar
                                :color="goal.priorityColor"
                                size="40"
                                class="mr-3"
                              >
                                <v-icon :color="'white'" size="20">
                                  {{ getPriorityIcon(goal.priority) }}
                                </v-icon>
                              </v-avatar>
                              <div class="flex-grow-1 min-width-0">
                                <h3 class="text-h6 font-weight-bold mb-1 text-truncate">
                                  {{ goal.name }}
                                </h3>
                                <div class="d-flex align-center text-body-2 text-medium-emphasis">
                                  <v-icon icon="mdi-tag" size="14" class="mr-1" />
                                  <span class="text-truncate">{{ goal.categoryName }}</span>
                                </div>
                              </div>
                            </div>
                            <v-chip
                              :color="goal.priorityColor"
                              size="small"
                              variant="flat"
                              class="ml-2"
                            >
                              {{ getPriorityLabel(goal.priority) }}
                            </v-chip>
                          </div>
                        </v-card-title>

                        <v-card-text class="pa-4">
                          <!-- Progress Section -->
                          <div class="mb-4">
                            <div class="d-flex justify-space-between align-center mb-3">
                              <span class="text-body-2 font-weight-medium text-medium-emphasis">Progress</span>
                              <div class="d-flex align-center">
                                <span class="text-h6 font-weight-bold mr-1" :class="`text-${goal.priorityColor}`">
                                  {{ goal.percentageComplete }}%
                                </span>
                                <v-chip
                                  :color="goal.statusColor"
                                  size="x-small"
                                  variant="flat"
                                  class="ml-1"
                                >
                                  {{ getStatusLabel(goal.status) }}
                                </v-chip>
                              </div>
                            </div>
                            <v-progress-linear
                              :model-value="goal.percentageComplete"
                              :color="goal.priorityColor"
                              height="12"
                              rounded="lg"
                              class="goal-progress-bar"
                            >
                              <template v-slot:default="{ value }">
                                <div class="progress-text">{{ Math.round(value) }}%</div>
                              </template>
                            </v-progress-linear>
                          </div>

                          <!-- Amount Cards -->
                          <v-row dense class="mb-3">
                            <v-col cols="4">
                              <v-card 
                                variant="flat" 
                                class="text-center pa-3 amount-card"
                                :style="{ backgroundColor: `rgba(var(--v-theme-${goal.priorityColor}), 0.08)` }"
                              >
                                <div class="text-caption text-medium-emphasis mb-1">Current</div>
                                <div class="text-body-1 font-weight-bold" :class="`text-${goal.priorityColor}`">
                                  {{ formatCurrency(goal.currentAmount) }}
                                </div>
                              </v-card>
                            </v-col>
                            <v-col cols="4">
                              <v-card 
                                variant="flat" 
                                class="text-center pa-3 amount-card"
                                :style="{ backgroundColor: `rgba(var(--v-theme-primary), 0.08)` }"
                              >
                                <div class="text-caption text-medium-emphasis mb-1">Target</div>
                                <div class="text-body-1 font-weight-bold text-primary">
                                  {{ formatCurrency(goal.targetAmount) }}
                                </div>
                              </v-card>
                            </v-col>
                            <v-col cols="4">
                              <v-card 
                                variant="flat" 
                                class="text-center pa-3 amount-card"
                                :style="{ backgroundColor: goal.remainingAmount > 0 ? 'rgba(var(--v-theme-info), 0.08)' : 'rgba(var(--v-theme-success), 0.08)' }"
                              >
                                <div class="text-caption text-medium-emphasis mb-1">Remaining</div>
                                <div 
                                  class="text-body-1 font-weight-bold"
                                  :class="goal.remainingAmount > 0 ? 'text-info' : 'text-success'"
                                >
                                  {{ formatCurrency(goal.remainingAmount) }}
                                </div>
                              </v-card>
                            </v-col>
                          </v-row>

                          <!-- Footer Info -->
                          <div class="d-flex justify-space-between align-center">
                            <div v-if="goal.tag" class="d-flex align-center">
                              <v-icon icon="mdi-label-outline" size="14" class="mr-1 text-medium-emphasis" />
                              <span class="text-caption text-medium-emphasis">{{ goal.tag }}</span>
                            </div>
                            <div v-else></div>
                            <div v-if="goal.endDate" class="d-flex align-center">
                              <v-icon 
                                :icon="goal.isOverdue ? 'mdi-alert-circle' : 'mdi-calendar-clock'" 
                                size="14" 
                                class="mr-1"
                                :class="goal.isOverdue ? 'text-error' : 'text-medium-emphasis'"
                              />
                              <span 
                                class="text-caption font-weight-medium"
                                :class="goal.isOverdue ? 'text-error' : 'text-medium-emphasis'"
                              >
                                <span v-if="goal.isOverdue">{{ Math.abs(goal.daysRemaining) }} days overdue</span>
                                <span v-else>{{ goal.daysRemaining }} days left</span>
                              </span>
                            </div>
                          </div>
                        </v-card-text>

                        <v-divider></v-divider>

                        <v-card-actions class="pa-3">
                          <v-spacer />
                          <v-btn
                            size="small"
                            variant="text"
                            prepend-icon="mdi-pencil"
                            @click="openEditDialog(goal)"
                            rounded="lg"
                          >
                            Edit
                          </v-btn>
                        </v-card-actions>
                      </v-card>
                    </v-col>
                  </v-row>
                </div>
              </v-window-item>

              <!-- Active Goals Tab -->
              <v-window-item value="active">
                <div v-if="isLoading" class="text-center py-8">
                  <v-progress-circular indeterminate color="primary" size="64" />
                  <p class="mt-4 text-medium-emphasis">Loading active goals...</p>
                </div>
                <div v-else-if="activeGoals.length === 0" class="text-center py-8">
                  <v-icon icon="mdi-play-circle" size="64" color="grey-lighten-1" class="mb-4" />
                  <h3 class="text-h5 mb-2">No Active Goals</h3>
                  <p class="text-medium-emphasis mb-4">Create your first goal to get started.</p>
                  <v-btn color="primary" @click="openCreateDialog">
                    Create Goal
                  </v-btn>
                </div>
                <v-data-table
                  v-else
                  :headers="activeGoalsHeaders"
                  :items="activeGoals"
                  :loading="isLoading"
                  class="elevation-1"
                >
                  <template v-slot:item.priority="{ item }">
                    <v-chip
                      :color="getPriorityColor(item.priority)"
                      size="small"
                      :prepend-icon="getPriorityIcon(item.priority)"
                    >
                      {{ getPriorityLabel(item.priority) }}
                    </v-chip>
                  </template>
                  <template v-slot:item.status="{ item }">
                    <v-chip
                      :color="getStatusColor(item.status)"
                      size="small"
                      :prepend-icon="getStatusIcon(item.status)"
                    >
                      {{ getStatusLabel(item.status) }}
                    </v-chip>
                  </template>
                  <template v-slot:item.targetAmount="{ item }">
                    {{ formatCurrency(item.targetAmount) }}
                  </template>
                  <template v-slot:item.currentAmount="{ item }">
                    {{ formatCurrency(item.currentAmount) }}
                  </template>
                  <template v-slot:item.progress="{ item }">
                    <div class="d-flex align-center">
                      <v-progress-linear
                        :model-value="(item.currentAmount / item.targetAmount) * 100"
                        :color="getProgressColor((item.currentAmount / item.targetAmount) * 100)"
                        height="6"
                        width="100"
                        rounded
                      />
                      <span class="ml-2 text-xs">{{ Math.round((item.currentAmount / item.targetAmount) * 100) }}%</span>
                    </div>
                  </template>
                  <template v-slot:item.actions="{ item }">
                    <v-btn
                      icon="mdi-pencil"
                      size="small"
                      variant="text"
                      @click="openEditDialog(item)"
                    />
                    <v-btn
                      icon="mdi-delete"
                      size="small"
                      variant="text"
                      color="error"
                      @click="deleteGoal(item.id)"
                    />
                  </template>
                </v-data-table>
              </v-window-item>

              <!-- Completed Goals Tab -->
              <v-window-item value="completed">
                <div v-if="isLoading" class="text-center py-8">
                  <v-progress-circular indeterminate color="primary" size="64" />
                  <p class="mt-4 text-medium-emphasis">Loading completed goals...</p>
                </div>
                <div v-else-if="completedGoals.length === 0" class="text-center py-8">
                  <v-icon icon="mdi-check-circle" size="64" color="grey-lighten-1" class="mb-4" />
                  <h3 class="text-h5 mb-2">No Completed Goals</h3>
                  <p class="text-medium-emphasis">Complete your first goal to see it here.</p>
                </div>
                <v-data-table
                  v-else
                  :headers="completedGoalsHeaders"
                  :items="completedGoals"
                  :loading="isLoading"
                  class="elevation-1"
                >
                  <template v-slot:item.priority="{ item }">
                    <v-chip
                      :color="getPriorityColor(item.priority)"
                      size="small"
                      :prepend-icon="getPriorityIcon(item.priority)"
                    >
                      {{ getPriorityLabel(item.priority) }}
                    </v-chip>
                  </template>
                  <template v-slot:item.targetAmount="{ item }">
                    {{ formatCurrency(item.targetAmount) }}
                  </template>
                  <template v-slot:item.currentAmount="{ item }">
                    {{ formatCurrency(item.currentAmount) }}
                  </template>
                  <template v-slot:item.completedAt="{ item }">
                    {{ formatDate(item.updatedAt) }}
                  </template>
                </v-data-table>
              </v-window-item>
            </v-window>
          </v-card-text>
        </v-card>
      </v-container>
    </v-main>

    <!-- Create/Edit Goal Dialog -->
    <v-dialog v-model="dialogOpen" max-width="600">
      <v-card>
        <v-card-title>
          {{ isEditing ? 'Edit Goal' : 'Create New Goal' }}
        </v-card-title>
        
        <v-card-text>
          <v-form ref="formRef" v-model="formValid">
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="editingGoal.name"
                  label="Goal Name"
                  :rules="[v => !!v || 'Name is required']"
                  required
                />
              </v-col>
              
              <v-col cols="12">
                <v-textarea
                  v-model="editingGoal.description"
                  label="Description"
                  rows="3"
                />
              </v-col>
              
              <v-col cols="6">
                <v-text-field
                  v-model.number="editingGoal.targetAmount"
                  label="Target Amount"
                  type="number"
                  :rules="[v => v > 0 || 'Target amount must be greater than 0']"
                  required
                />
              </v-col>
              
              <v-col cols="6">
                <v-text-field
                  v-model.number="editingGoal.currentAmount"
                  label="Current Amount"
                  type="number"
                  :rules="[v => v >= 0 || 'Current amount cannot be negative']"
                  required
                />
              </v-col>
              
              <v-col cols="12">
                <v-select
                  v-model="editingGoal.categoryId"
                  :items="savingsCategories"
                  item-title="name"
                  item-value="id"
                  label="Category"
                  :rules="[v => !!v || 'Category is required']"
                  required
                />
              </v-col>
              
              <v-col cols="6">
                <v-text-field
                  v-model="editingGoal.startDate"
                  label="Start Date"
                  type="date"
                  :rules="[v => !!v || 'Start date is required']"
                  required
                />
              </v-col>
              
              <v-col cols="6">
                <v-text-field
                  v-model="editingGoal.endDate"
                  label="End Date (Optional)"
                  type="date"
                />
              </v-col>
              
              <v-col cols="6">
                <v-select
                  v-model="editingGoal.priority"
                  :items="priorityOptions"
                  item-title="text"
                  item-value="value"
                  label="Priority"
                  required
                />
              </v-col>
              
              <v-col cols="6">
                <v-select
                  v-model="editingGoal.status"
                  :items="statusOptions"
                  item-title="text"
                  item-value="value"
                  label="Status"
                  required
                />
              </v-col>
              
              <v-col cols="12">
                <v-text-field
                  v-model="editingGoal.tag"
                  label="Tag (Optional)"
                  placeholder="e.g., Emergency Fund, Vacation"
                />
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        
        <v-card-actions>
          <v-spacer />
          <v-btn @click="closeDialog">Cancel</v-btn>
          <v-btn
            color="primary"
            :loading="isSubmitting"
            :disabled="!formValid"
            @click="saveGoal"
          >
            {{ isEditing ? 'Update' : 'Create' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <AppFooter />
  </v-app>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import { goalService } from '@/lib/goalService'
import { categoryService } from '@/lib/categoryService'
import type { 
  GoalDto, 
  GoalProgressDto, 
  CreateGoalDto, 
  UpdateGoalDto
} from '@/types/goal'
import type { CategoryDto } from '@/types/category'
import { 
  GoalStatus,
  GoalPriority
} from '@/types/goal'

// Reactive state
const activeTab = ref('progress')
const isLoading = ref(false)
const isSubmitting = ref(false)
const dialogOpen = ref(false)
const isEditing = ref(false)
const formValid = ref(false)
const formRef = ref()

// Data
const goalProgress = ref<GoalProgressDto[]>([])
const activeGoals = ref<GoalDto[]>([])
const completedGoals = ref<GoalDto[]>([])
const savingsCategories = ref<CategoryDto[]>([])

// Form data
const editingGoal = ref<CreateGoalDto | UpdateGoalDto>({
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

// Table headers
const activeGoalsHeaders = [
  { title: 'Name', key: 'name' },
  { title: 'Category', key: 'category.name' },
  { title: 'Priority', key: 'priority' },
  { title: 'Status', key: 'status' },
  { title: 'Target', key: 'targetAmount' },
  { title: 'Current', key: 'currentAmount' },
  { title: 'Progress', key: 'progress' },
  { title: 'Actions', key: 'actions', sortable: false }
]

const completedGoalsHeaders = [
  { title: 'Name', key: 'name' },
  { title: 'Category', key: 'category.name' },
  { title: 'Priority', key: 'priority' },
  { title: 'Target', key: 'targetAmount' },
  { title: 'Achieved', key: 'currentAmount' },
  { title: 'Completed', key: 'completedAt' }
]

// Options
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

const toast = useToast()

// Methods
const loadData = async () => {
  isLoading.value = true
  try {
    await Promise.all([
      loadGoalProgress(),
      loadActiveGoals(),
      loadCompletedGoals(),
      loadSavingsCategories()
    ])
  } catch (error) {
    console.error('Failed to load goals data:', error)
    toast.error('Failed to load goals data')
  } finally {
    isLoading.value = false
  }
}

const loadGoalProgress = async () => {
  try {
    goalProgress.value = await goalService.getProgress()
  } catch (error) {
    console.error('Failed to load goal progress:', error)
    goalProgress.value = []
  }
}

const loadActiveGoals = async () => {
  try {
    activeGoals.value = await goalService.getActive()
  } catch (error) {
    console.error('Failed to load active goals:', error)
    activeGoals.value = []
  }
}

const loadCompletedGoals = async () => {
  try {
    completedGoals.value = await goalService.getCompleted()
  } catch (error) {
    console.error('Failed to load completed goals:', error)
    completedGoals.value = []
  }
}

const loadSavingsCategories = async () => {
  try {
    const categories = await categoryService.list()
    const allSavingsCategories = categories.filter((cat: CategoryDto) => cat.categoryType === 'TargetedSavingsGoal')
    
    // Get active goals to filter out categories that already have active goals
    const activeGoalsList = await goalService.getActive()
    const activeCategoryIds = new Set(activeGoalsList.map(goal => goal.categoryId))
    
    // Filter out categories that already have active goals (unless we're editing that goal)
    if (isEditing.value && 'id' in editingGoal.value && editingGoal.value.id) {
      const goalId = editingGoal.value.id
      const currentGoal = activeGoalsList.find(g => g.id === goalId)
      if (currentGoal) {
        // Allow the current goal's category
        savingsCategories.value = allSavingsCategories.filter(cat => 
          !activeCategoryIds.has(cat.id) || cat.id === currentGoal.categoryId
        )
      } else {
        savingsCategories.value = allSavingsCategories.filter(cat => !activeCategoryIds.has(cat.id))
      }
    } else {
      // Creating new goal - exclude all categories with active goals
      savingsCategories.value = allSavingsCategories.filter(cat => !activeCategoryIds.has(cat.id))
    }
  } catch (error) {
    console.error('Failed to load categories:', error)
    savingsCategories.value = []
  }
}

const openCreateDialog = async () => {
  isEditing.value = false
  editingGoal.value = {
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
  }
  // Reload categories to filter out those with active goals
  await loadSavingsCategories()
  dialogOpen.value = true
}

const openEditDialog = async (goal: GoalDto | GoalProgressDto) => {
  isEditing.value = true
  editingGoal.value = {
    id: 'goalId' in goal ? goal.goalId : goal.id,
    name: goal.name,
    description: goal.description || '',
    targetAmount: goal.targetAmount,
    currentAmount: goal.currentAmount,
    categoryId: goal.categoryId,
    startDate: goal.startDate.split('T')[0],
    endDate: goal.endDate ? goal.endDate.split('T')[0] : null,
    tag: goal.tag || '',
    status: goal.status,
    priority: goal.priority
  }
  // Reload categories to filter out those with active goals (allowing current goal's category)
  await loadSavingsCategories()
  dialogOpen.value = true
}

const closeDialog = () => {
  dialogOpen.value = false
  formRef.value?.reset()
}

const saveGoal = async () => {
  if (!formValid.value) return
  
  isSubmitting.value = true
  try {
    if (isEditing.value) {
      await goalService.update(editingGoal.value as UpdateGoalDto)
      toast.success('Goal updated successfully')
    } else {
      await goalService.create(editingGoal.value as CreateGoalDto)
      toast.success('Goal created successfully')
    }
    
    closeDialog()
    await loadData()
  } catch (error) {
    console.error('Failed to save goal:', error)
    toast.error('Failed to save goal')
  } finally {
    isSubmitting.value = false
  }
}

const deleteGoal = async (id: string) => {
  if (!confirm('Are you sure you want to delete this goal?')) return
  
  try {
    await goalService.delete(id)
    toast.success('Goal deleted successfully')
    await loadData()
  } catch (error) {
    console.error('Failed to delete goal:', error)
    toast.error('Failed to delete goal')
  }
}

// Helper functions
const getProgressColor = (percentage: number) => {
  if (percentage >= 100) return 'success'
  if (percentage >= 80) return 'warning'
  if (percentage >= 50) return 'info'
  return 'primary'
}

const getStatusIcon = (status: GoalStatus) => {
  switch (status) {
    case GoalStatus.Active: return 'mdi-play-circle'
    case GoalStatus.Paused: return 'mdi-pause-circle'
    case GoalStatus.Completed: return 'mdi-check-circle'
    case GoalStatus.Cancelled: return 'mdi-cancel'
    default: return 'mdi-help'
  }
}

const getPriorityIcon = (priority: GoalPriority) => {
  switch (priority) {
    case GoalPriority.Low: return 'mdi-arrow-down'
    case GoalPriority.Medium: return 'mdi-minus'
    case GoalPriority.High: return 'mdi-arrow-up'
    default: return 'mdi-help'
  }
}

const getPriorityColor = (priority: GoalPriority) => {
  switch (priority) {
    case GoalPriority.Low: return 'success'
    case GoalPriority.Medium: return 'warning'
    case GoalPriority.High: return 'error'
    default: return 'default'
  }
}

const getPriorityLabel = (priority: GoalPriority) => {
  switch (priority) {
    case GoalPriority.Low: return 'Low'
    case GoalPriority.Medium: return 'Medium'
    case GoalPriority.High: return 'High'
    default: return 'Unknown'
  }
}

const getStatusColor = (status: GoalStatus) => {
  switch (status) {
    case GoalStatus.Active: return 'success'
    case GoalStatus.Paused: return 'warning'
    case GoalStatus.Completed: return 'info'
    case GoalStatus.Cancelled: return 'error'
    default: return 'default'
  }
}

const getStatusLabel = (status: GoalStatus) => {
  switch (status) {
    case GoalStatus.Active: return 'Active'
    case GoalStatus.Paused: return 'Paused'
    case GoalStatus.Completed: return 'Completed'
    case GoalStatus.Cancelled: return 'Cancelled'
    default: return 'Unknown'
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(amount)
}

const formatDate = (date: string | Date) => {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  return dateObj.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

// Lifecycle
onMounted(() => {
  loadData()
})
</script>

<style scoped>
.goal-progress-container {
  padding: 8px 0;
}

.goal-progress-card {
  transition: all 0.3s ease;
  border: 1px solid rgba(var(--v-theme-outline), 0.12);
}

.goal-progress-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(var(--v-theme-primary), 0.15) !important;
  border-color: rgba(var(--v-theme-primary), 0.3);
}

.goal-card-header {
  border-bottom: 1px solid rgba(var(--v-theme-outline), 0.12);
}

.goal-progress-bar {
  position: relative;
  overflow: visible;
}

.goal-progress-bar :deep(.v-progress-linear__content) {
  position: relative;
}

.progress-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 10px;
  font-weight: 600;
  color: white;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
  z-index: 1;
}

.amount-card {
  transition: all 0.2s ease;
  border: 1px solid rgba(var(--v-theme-outline), 0.08);
  min-height: 70px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.amount-card:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(var(--v-theme-primary), 0.1);
}

.min-width-0 {
  min-width: 0;
}
</style>
