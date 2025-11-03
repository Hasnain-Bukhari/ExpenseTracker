<template>
  <v-card class="goals-card dashboard-card">
    <!-- Card Header -->
    <v-card-title class="d-flex align-center justify-space-between pa-4">
      <div class="d-flex align-center">
        <v-icon color="primary" class="mr-2" size="20">mdi-target</v-icon>
        <span class="text-body-2 font-weight-medium">Financial Goals</span>
      </div>
      <v-btn
        variant="text"
        size="small"
        color="primary"
        @click="viewAllGoals"
      >
        View All
        <v-icon right>mdi-arrow-right</v-icon>
      </v-btn>
    </v-card-title>

    <!-- Goals List -->
    <v-card-text class="pa-0">
      <div v-if="isLoading" class="text-center py-6">
        <v-progress-circular indeterminate color="primary" size="32"></v-progress-circular>
        <p class="text-caption text-medium-emphasis mt-2">Loading goals...</p>
      </div>

      <div v-else-if="goals.length === 0" class="text-center py-6">
        <v-icon size="48" color="grey-lighten-1" class="mb-2">mdi-target-outline</v-icon>
        <p class="text-body-2 text-medium-emphasis mb-2">No active goals</p>
        <p class="text-caption text-medium-emphasis">Create your first goal to track progress</p>
      </div>

      <div v-else class="goals-list">
        <div v-for="goal in goals.slice(0, 2)" :key="goal.goalId" class="goal-item mb-3">
          <!-- Goal Card with Gradient Header -->
          <v-card 
            class="goal-progress-card" 
            elevation="2"
            rounded="lg"
            variant="outlined"
          >
            <!-- Card Header with gradient background -->
            <v-card-title 
              class="goal-card-header pa-3 pb-2"
              :style="{ background: `linear-gradient(135deg, rgba(var(--v-theme-${goal.priorityColor}), 0.1) 0%, rgba(var(--v-theme-${goal.priorityColor}), 0.05) 100%)` }"
            >
              <div class="d-flex align-center justify-space-between w-100">
                <div class="d-flex align-center flex-grow-1 min-width-0">
                  <v-avatar
                    :color="goal.priorityColor"
                    size="32"
                    class="mr-2"
                  >
                    <v-icon :color="'white'" size="16">
                      {{ getPriorityIcon(goal.priority) }}
                    </v-icon>
                  </v-avatar>
                  <div class="flex-grow-1 min-width-0">
                    <h3 class="text-body-2 font-weight-bold mb-0 text-truncate">
                      {{ goal.name }}
                    </h3>
                    <div class="d-flex align-center text-caption text-medium-emphasis">
                      <v-icon icon="mdi-tag" size="12" class="mr-1" />
                      <span class="text-truncate">{{ goal.categoryName }}</span>
                    </div>
                  </div>
                </div>
                <v-chip
                  :color="goal.priorityColor"
                  size="x-small"
                  variant="flat"
                  class="ml-2"
                >
                  {{ getPriorityLabel(goal.priority) }}
                </v-chip>
              </div>
            </v-card-title>

            <v-card-text class="pa-3">
              <!-- Progress Section -->
              <div class="mb-3">
                <div class="d-flex justify-space-between align-center mb-2">
                  <span class="text-caption font-weight-medium text-medium-emphasis">Progress</span>
                  <div class="d-flex align-center">
                    <span class="text-body-2 font-weight-bold mr-1" :class="`text-${goal.priorityColor}`">
                      {{ goal.percentageComplete }}%
                    </span>
                    <v-chip
                      :color="goal.statusColor"
                      size="x-small"
                      variant="flat"
                    >
                      {{ getStatusLabel(goal.status) }}
                    </v-chip>
                  </div>
                </div>
                <v-progress-linear
                  :model-value="goal.percentageComplete"
                  :color="goal.priorityColor"
                  height="10"
                  rounded="lg"
                  class="goal-progress-bar"
                >
                  <template v-slot:default="{ value }">
                    <div class="progress-text">{{ Math.round(value) }}%</div>
                  </template>
                </v-progress-linear>
              </div>

              <!-- Amount Cards -->
              <v-row dense class="mb-2">
                <v-col cols="4" class="pa-1">
                  <v-card 
                    variant="flat" 
                    class="text-center pa-2 amount-card"
                    :style="{ backgroundColor: `rgba(var(--v-theme-${goal.priorityColor}), 0.08)` }"
                  >
                    <div class="text-caption text-medium-emphasis mb-1">Current</div>
                    <div class="text-body-2 font-weight-bold" :class="`text-${goal.priorityColor}`">
                      {{ formatCurrencyCompact(goal.currentAmount) }}
                    </div>
                  </v-card>
                </v-col>
                <v-col cols="4" class="pa-1">
                  <v-card 
                    variant="flat" 
                    class="text-center pa-2 amount-card"
                    :style="{ backgroundColor: `rgba(var(--v-theme-primary), 0.08)` }"
                  >
                    <div class="text-caption text-medium-emphasis mb-1">Target</div>
                    <div class="text-body-2 font-weight-bold text-primary">
                      {{ formatCurrencyCompact(goal.targetAmount) }}
                    </div>
                  </v-card>
                </v-col>
                <v-col cols="4" class="pa-1">
                  <v-card 
                    variant="flat" 
                    class="text-center pa-2 amount-card"
                    :style="{ backgroundColor: goal.remainingAmount > 0 ? 'rgba(var(--v-theme-info), 0.08)' : 'rgba(var(--v-theme-success), 0.08)' }"
                  >
                    <div class="text-caption text-medium-emphasis mb-1">Left</div>
                    <div 
                      class="text-body-2 font-weight-bold"
                      :class="goal.remainingAmount > 0 ? 'text-info' : 'text-success'"
                    >
                      {{ formatCurrencyCompact(goal.remainingAmount) }}
                    </div>
                  </v-card>
                </v-col>
              </v-row>

              <!-- Footer Info -->
              <div v-if="goal.endDate || goal.tag" class="d-flex justify-space-between align-center mt-2">
                <div v-if="goal.tag" class="d-flex align-center">
                  <v-icon icon="mdi-label-outline" size="12" class="mr-1 text-medium-emphasis" />
                  <span class="text-caption text-medium-emphasis">{{ goal.tag }}</span>
                </div>
                <div v-else></div>
                <div v-if="goal.endDate" class="d-flex align-center">
                  <v-icon 
                    :icon="goal.isOverdue ? 'mdi-alert-circle' : 'mdi-calendar-clock'" 
                    size="12" 
                    class="mr-1"
                    :class="goal.isOverdue ? 'text-error' : 'text-medium-emphasis'"
                  />
                  <span 
                    class="text-caption font-weight-medium"
                    :class="goal.isOverdue ? 'text-error' : 'text-medium-emphasis'"
                  >
                    <span v-if="goal.isOverdue">{{ Math.abs(goal.daysRemaining) }}d overdue</span>
                    <span v-else>{{ goal.daysRemaining }}d left</span>
                  </span>
                </div>
              </div>
            </v-card-text>
          </v-card>
        </div>

        <div v-if="goals.length > 2" class="text-center mt-3 pa-3">
          <v-btn
            variant="text"
            size="small"
            color="primary"
            @click="viewAllGoals"
          >
            View {{ goals.length - 2 }} more goals
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
import { formatCurrency } from '@/utils/formatters'
import type { GoalProgressDto } from '@/types/goal'
import { GoalPriority, GoalStatus, getGoalPriorityLabel } from '@/types/goal'

const router = useRouter()
const toast = useToast()
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

// Helper functions
const getPriorityIcon = (priority: GoalPriority) => {
  switch (priority) {
    case GoalPriority.Low: return 'mdi-arrow-down'
    case GoalPriority.Medium: return 'mdi-minus'
    case GoalPriority.High: return 'mdi-arrow-up'
    default: return 'mdi-help'
  }
}

const getPriorityLabel = (priority: GoalPriority): string => {
  return getGoalPriorityLabel(priority)
}

const getStatusLabel = (status: GoalStatus): string => {
  switch (status) {
    case GoalStatus.Active: return 'Active'
    case GoalStatus.Paused: return 'Paused'
    case GoalStatus.Completed: return 'Completed'
    case GoalStatus.Cancelled: return 'Cancelled'
    default: return 'Unknown'
  }
}

const formatCurrencyCompact = (amount: number): string => {
  if (amount >= 1000000) {
    return `$${(amount / 1000000).toFixed(1)}M`
  } else if (amount >= 1000) {
    return `$${(amount / 1000).toFixed(1)}K`
  }
  return formatCurrency(amount)
}

// Actions
const viewAllGoals = () => {
  router.push('/goals')
}

onMounted(() => {
  loadGoals()
})
</script>

<style scoped>
.goals-card {
  height: 100%;
}

.goals-card .v-card-title {
  border-bottom: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
}

.goals-list {
  padding: 8px;
}

.goal-progress-card {
  transition: all 0.3s ease;
  border: 1px solid rgba(var(--v-theme-outline), 0.12);
}

.goal-progress-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(var(--v-theme-primary), 0.15) !important;
  border-color: rgba(var(--v-theme-primary), 0.3);
}

.goal-card-header {
  border-bottom: 1px solid rgba(var(--v-theme-outline), 0.08);
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
  border: 1px solid rgba(var(--v-theme-outline), 0.06);
  min-height: 50px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.amount-card:hover {
  transform: scale(1.05);
  box-shadow: 0 2px 8px rgba(var(--v-theme-primary), 0.1);
}

.min-width-0 {
  min-width: 0;
}
</style>
