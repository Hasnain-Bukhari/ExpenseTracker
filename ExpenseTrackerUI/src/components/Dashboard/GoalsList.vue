<template>
  <v-card class="goals-card dashboard-card">
    <!-- Card Header -->
    <v-card-title class="goals-header pa-4 pa-sm-6 pb-4">
      <div class="header-content w-100 d-flex align-center justify-space-between">
        <div class="header-main d-flex align-center">
          <v-avatar 
            :size="$vuetify.display.mobile ? 36 : 40" 
            color="primary" 
            variant="tonal"
            class="mr-3"
          >
            <v-icon icon="mdi-target" :size="$vuetify.display.mobile ? 18 : 20" />
          </v-avatar>
          <div class="header-text">
            <h3 class="text-body-1 text-sm-h6 font-weight-bold mb-0">Financial Goals</h3>
            <p class="text-caption text-sm-body-2 text-secondary mb-0 d-none d-sm-block">Track your progress</p>
          </div>
        </div>
        
        <v-btn
          variant="text"
          :size="$vuetify.display.mobile ? 'x-small' : 'small'"
          color="primary"
          @click="viewAllGoals"
        >
          <span :class="$vuetify.display.mobile ? 'text-caption' : 'text-body-2'">View All</span>
        </v-btn>
      </div>
    </v-card-title>

    <!-- Goals List -->
    <v-card-text class="pa-0">
      <div class="goals-container pa-3 pa-sm-6 pt-0">
        <div v-if="isLoading" class="text-center py-8">
          <v-progress-circular indeterminate color="primary" size="32" />
          <p class="mt-2 text-caption text-medium-emphasis">Loading goals...</p>
        </div>
        
        <div
          v-else-if="goals.length > 0"
          v-for="(goal, index) in goals.slice(0, 3)"
          :key="goal.goalId"
          class="goal-item"
          v-motion
          :initial="{ opacity: 0, x: -20 }"
          :enter="{ 
            opacity: 1, 
            x: 0, 
            transition: { 
              delay: index * 100,
              type: 'spring',
              stiffness: 200
            } 
          }"
        >
          <!-- Goal Header -->
          <div class="goal-header d-flex align-center justify-space-between mb-3">
            <div class="goal-info flex-grow-1">
              <div class="d-flex align-center mb-1">
                <h4 class="goal-title text-subtitle-1 font-weight-semibold mr-2">
                  {{ goal.name }}
                </h4>
                <v-chip
                  :color="getPriorityColor(goal.priority)"
                  size="x-small"
                  variant="tonal"
                  class="priority-chip"
                >
                  {{ getPriorityLabel(goal.priority) }}
                </v-chip>
              </div>
              
              <div class="goal-amounts d-flex align-center">
                <span class="current-amount text-h6 font-weight-bold text-primary mr-2">
                  {{ formatCurrency(goal.currentAmount) }}
                </span>
                <span class="amount-separator text-body-2 text-tertiary mr-2">of</span>
                <span class="target-amount text-body-2 font-weight-medium text-secondary">
                  {{ formatCurrency(goal.targetAmount) }}
                </span>
              </div>
            </div>
            
            <!-- Goal Actions -->
            <div class="goal-actions">
              <v-btn
                icon
                size="small"
                variant="text"
                @click="quickAddToGoal(goal.goalId)"
                class="action-btn"
              >
                <v-icon size="16">mdi-plus</v-icon>
                <v-tooltip activator="parent" location="top">
                  Quick Add
                </v-tooltip>
              </v-btn>
            </div>
          </div>

          <!-- Progress Section -->
          <div class="progress-section mb-3">
            <!-- Progress Bar -->
            <div class="progress-container mb-2">
              <v-progress-linear
                :model-value="goal.percentageComplete"
                :color="getProgressColor(goal.percentageComplete)"
                height="12"
                rounded
                class="progress-bar"
                :class="{ 'progress-animated': isAnimating }"
              >
                <template v-slot:default>
                  <div class="progress-content d-flex align-center justify-center h-100">
                    <span class="progress-text text-caption font-weight-medium">
                      {{ goal.percentageComplete }}%
                    </span>
                  </div>
                </template>
              </v-progress-linear>
            </div>
            
            <!-- Progress Details -->
            <div class="progress-details d-flex justify-space-between align-center">
              <div class="progress-stats">
                <span class="text-body-2 text-secondary">
                  {{ goal.percentageComplete }}% complete
                </span>
                <span class="remaining-amount text-caption text-tertiary ml-2">
                  ({{ formatCurrency(goal.remainingAmount) }} remaining)
                </span>
              </div>
              
              <div v-if="goal.endDate" class="deadline-info d-flex align-center">
                <v-icon 
                  :color="getDeadlineUrgency(goal.endDate)"
                  size="14" 
                  class="mr-1"
                >
                  mdi-calendar-clock
                </v-icon>
                <span 
                  :class="`text-caption text-${getDeadlineUrgency(goal.endDate)}`"
                >
                  {{ formatDateShort(goal.endDate) }}
                </span>
              </div>
            </div>
          </div>

          <!-- Goal Category & Status -->
          <div class="goal-footer d-flex align-center justify-space-between">
            <div class="goal-category">
              <v-chip
                size="small"
                variant="outlined"
                class="category-chip"
              >
                <v-icon start size="12">mdi-tag-outline</v-icon>
                {{ goal.categoryName }}
              </v-chip>
            </div>
            
            <div class="goal-status">
              <v-chip
                :color="goal.statusColor"
                size="small"
                variant="tonal"
                class="status-chip"
              >
                {{ getStatusText(goal.percentageComplete) }}
              </v-chip>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="empty-state text-center py-8">
          <v-icon size="64" color="primary" class="mb-4">mdi-target</v-icon>
          <h3 class="text-h6 font-weight-medium mb-2">No Goals Yet</h3>
          <p class="text-body-2 text-secondary mb-4">
            Set your first financial goal to start tracking your progress
          </p>
          <v-btn color="primary" @click="addNewGoal">
            <v-icon start>mdi-plus</v-icon>
            Add Your First Goal
          </v-btn>
        </div>

      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'
import { goalService } from '@/lib/goalService'
import { formatCurrency, formatDateShort } from '@/utils/formatters'
import type { GoalProgressDto, GoalPriority } from '@/types/goal'
import { getGoalPriorityLabel, getGoalPriorityColor } from '@/types/goal'

const router = useRouter()
const toast = useToast()
const isAnimating = ref(true)
const isLoading = ref(false)
const goals = ref<GoalProgressDto[]>([])

// Load goals data
const loadGoals = async () => {
  isLoading.value = true
  try {
    goals.value = await goalService.getProgress()
  } catch (error) {
    console.error('Failed to load goals:', error)
    toast.error('Failed to load goals')
    goals.value = []
  } finally {
    isLoading.value = false
  }
}

const getProgressColor = (percentage: number): string => {
  if (percentage >= 90) return 'success'
  if (percentage >= 70) return 'primary'
  if (percentage >= 40) return 'warning'
  return 'error'
}

const getPriorityColor = (priority: GoalPriority): string => {
  return getGoalPriorityColor(priority)
}

const getPriorityLabel = (priority: GoalPriority): string => {
  return getGoalPriorityLabel(priority)
}

// Status helpers
const getStatusText = (percentage: number): string => {
  if (percentage >= 100) return 'Completed'
  if (percentage >= 75) return 'Nearly There'
  if (percentage >= 50) return 'On Track'
  if (percentage >= 25) return 'Getting Started'
  return 'Just Started'
}

// Deadline helpers
const getDeadlineUrgency = (deadline: string): string => {
  const deadlineDate = new Date(deadline)
  const today = new Date()
  const daysUntilDeadline = Math.ceil((deadlineDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24))
  
  if (daysUntilDeadline < 0) return 'error'
  if (daysUntilDeadline <= 30) return 'warning'
  if (daysUntilDeadline <= 90) return 'primary'
  return 'success'
}

// Actions
const addNewGoal = () => {
  router.push('/goals')
}

const viewAllGoals = () => {
  router.push('/goals')
}

const quickAddToGoal = (goalId: string) => {
  // TODO: Open quick add modal
  console.log('Quick add to goal:', goalId)
}

onMounted(() => {
  loadGoals()
  // Start animation after component mounts
  setTimeout(() => {
    isAnimating.value = false
  }, 2000)
})
</script>
