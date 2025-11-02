<template>
  <v-card class="budget-panel" elevation="2">
    <v-card-title class="d-flex align-center justify-space-between pa-4">
      <div class="d-flex align-center">
        <v-icon color="primary" class="mr-2">mdi-chart-pie</v-icon>
        <span class="text-h6 font-weight-medium">Budget Progress</span>
      </div>
      <v-btn
        variant="text"
        size="small"
        color="primary"
        @click="navigateToBudgets"
      >
        View All
        <v-icon right>mdi-arrow-right</v-icon>
      </v-btn>
    </v-card-title>

    <v-card-text class="pa-4">
      <div v-if="isLoading" class="text-center py-4">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
        <p class="text-caption text-medium-emphasis mt-2">Loading budgets...</p>
      </div>

      <div v-else-if="budgets.length === 0" class="text-center py-4">
        <v-icon size="48" color="grey-lighten-1" class="mb-2">mdi-chart-pie-outline</v-icon>
        <p class="text-body-2 text-medium-emphasis mb-2">No active budgets</p>
        <p class="text-caption text-medium-emphasis">Create your first budget to track spending</p>
      </div>

      <div v-else>
        <div v-for="budget in budgets.slice(0, 10)" :key="budget.budgetId" class="mb-4">
          <div class="d-flex align-center justify-space-between mb-2">
            <span class="text-body-2 font-weight-medium">{{ budget.categoryName }}</span>
            <span class="text-caption text-medium-emphasis">{{ budget.percentageUsed }}%</span>
          </div>
          
          <v-progress-linear
            :model-value="budget.percentageUsed"
            :color="getProgressColor(budget.percentageUsed)"
            height="6"
            rounded
            class="mb-1"
          ></v-progress-linear>
          
          <div class="d-flex justify-space-between text-caption text-medium-emphasis">
            <span>${{ formatCurrency(budget.spentAmount) }} / ${{ formatCurrency(budget.allocatedAmount) }}</span>
            <span :class="getStatusTextColor(budget.statusColor)">{{ budget.status }}</span>
          </div>
        </div>

        <div v-if="budgets.length > 10" class="text-center mt-3">
          <v-btn
            variant="text"
            size="small"
            color="primary"
            @click="navigateToBudgets"
          >
            View {{ budgets.length - 10 }} more budgets
          </v-btn>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { budgetService } from '@/lib/budgetService'
import type { BudgetStatusDto } from '@/types/budget'

const router = useRouter()
const budgets = ref<BudgetStatusDto[]>([])
const isLoading = ref(false)

const loadBudgets = async () => {
  try {
    isLoading.value = true
    const data = await budgetService.getStatuses()
    budgets.value = data
  } catch (error) {
    console.error('Failed to load budgets:', error)
  } finally {
    isLoading.value = false
  }
}

const navigateToBudgets = () => {
  router.push('/budgets')
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const getProgressColor = (percentage: number): string => {
  if (percentage >= 100) return 'error'
  if (percentage >= 80) return 'warning'
  if (percentage >= 40) return 'success'
  return 'info'
}

const getStatusTextColor = (statusColor: string): string => {
  switch (statusColor) {
    case 'error': return 'text-error'
    case 'warning': return 'text-warning'
    case 'success': return 'text-success'
    default: return 'text-info'
  }
}

onMounted(() => {
  loadBudgets()
})
</script>

<style scoped>
.budget-panel {
  height: 100%;
}

.budget-panel .v-card-title {
  border-bottom: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
}
</style>
