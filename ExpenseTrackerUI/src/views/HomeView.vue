<template>
  <v-app>
    <AppHeader />
    <AppNav />
    
    <v-main class="main-content">
      <div class="container-custom pa-3 pa-sm-6">
        <!-- Welcome Section -->
        <div 
          class="welcome-section mb-6 mb-sm-8"
          v-motion
          :initial="{ opacity: 0, y: -20 }"
          :enter="{ opacity: 1, y: 0, transition: { duration: 500 } }"
        >
          <div class="welcome-content">
            <div class="welcome-text">
              <h1 class="welcome-title text-h4 text-sm-h3 font-weight-bold mb-2">
                Welcome back, Han! 👋
              </h1>
              <p class="welcome-subtitle text-subtitle-1 text-sm-h6 text-secondary mb-3 mb-sm-0">
                Here's what's happening with your finances today.
              </p>
            </div>
            
            <!-- Quick Actions -->
            <div class="quick-actions d-flex flex-column flex-sm-row align-stretch align-sm-center gap-2">
              <v-btn
                color="primary"
                variant="elevated"
                :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-plus'"
                :size="$vuetify.display.mobile ? 'small' : 'default'"
                @click="openTransactionDialog()"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-plus</v-icon>
                Add Transaction
              </v-btn>
              
              <v-btn
                color="success"
                variant="tonal"
                :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-target'"
                :size="$vuetify.display.mobile ? 'small' : 'default'"
                @click="openGoalDialog()"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-target</v-icon>
                New Goal
              </v-btn>
              
              <v-btn
                color="info"
                variant="tonal"
                :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-chart-line'"
                :size="$vuetify.display.mobile ? 'small' : 'default'"
                @click="openBudgetDialog()"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-chart-line</v-icon>
                New Budget
              </v-btn>
            </div>
          </div>
          
          <!-- Quick Stats Bar -->
          <div class="quick-stats-bar mt-6">
            <v-row>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Today's Spending</p>
                  <p class="text-h6 font-weight-bold text-primary mb-0">{{ formatCurrency(todaySpending) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Monthly Budget Left</p>
                  <p class="text-h6 font-weight-bold text-success mb-0">{{ formatCurrency(monthlyBudgetRemaining) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Goals Progress</p>
                  <p class="text-h6 font-weight-bold text-warning mb-0">{{ Math.round(goalsProgressPercentage) }}%</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Spending vs Budget</p>
                  <p class="text-h6 font-weight-bold mb-0" :class="getBudgetScoreColor(spendingVsBudgetScore)">{{ spendingVsBudgetScore }}</p>
                </div>
              </v-col>
            </v-row>
          </div>
        </div>

        <!-- Summary Cards -->
        <div 
          class="summary-section mb-8"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 200, duration: 500 } }"
        >
          <SummaryCards />
        </div>

        <!-- Charts and Goals Row -->
        <div 
          class="analytics-section mb-8"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 400, duration: 500 } }"
        >
          <v-row>
            <v-col cols="12" lg="6">
              <MiniChart />
            </v-col>
            <v-col cols="12" lg="3">
              <GoalsList />
            </v-col>
            <v-col cols="12" lg="3">
              <BudgetPanel />
            </v-col>
          </v-row>
        </div>

        <!-- Calendar Panel -->
        <div 
          class="calendar-section mb-8"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 500, duration: 500 } }"
        >
          <v-row>
            <v-col cols="12" md="6">
              <CalendarPanel />
            </v-col>
            <v-col cols="12" md="6">
              <!-- Placeholder for second panel -->
              <v-card class="dashboard-card glass-card" style="height: 400px;">
                <v-card-text class="d-flex align-center justify-center h-100">
                  <div class="text-center">
                    <v-icon icon="mdi-plus" size="48" color="primary" class="mb-4"></v-icon>
                    <h3 class="text-h6 mb-2">Add Another Panel</h3>
                    <p class="text-body-2 text-secondary">Space for additional dashboard content</p>
                  </div>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </div>

        <!-- Recent Transactions -->
        <div 
          class="transactions-section"
          v-motion
          :initial="{ opacity: 0, y: 20 }"
          :enter="{ opacity: 1, y: 0, transition: { delay: 600, duration: 500 } }"
        >
          <RecentTransactions />
        </div>

        <!-- Floating Action Button (Mobile) -->
        <v-fab
          v-if="isMobile"
          color="primary"
          icon="mdi-plus"
          location="bottom end"
          size="large"
          @click="showQuickActions = true"
          class="fab-main"
        />
      </div>
    </v-main>

    <AppFooter />

    <!-- Mobile Floating Action Button -->
    <div v-if="$vuetify.display.mobile" class="fab-container">
      <v-menu
        v-model="showQuickActions"
        :close-on-content-click="false"
        location="top"
        origin="bottom"
      >
        <template v-slot:activator="{ props }">
          <v-btn
            v-bind="props"
            color="primary"
            icon
            size="large"
            elevation="8"
            class="fab-main"
          >
            <v-icon :class="{ 'rotate-45': showQuickActions }">
              {{ showQuickActions ? 'mdi-close' : 'mdi-plus' }}
            </v-icon>
          </v-btn>
        </template>
        
        <v-card class="fab-menu" elevation="8">
          <v-list density="compact">
            <v-list-item
              prepend-icon="mdi-plus-circle-outline"
              title="Add Transaction"
              @click="openTransactionDialog(); showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-target"
              title="New Goal"
              @click="openGoalDialog(); showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-chart-line"
              title="New Budget"
              @click="openBudgetDialog(); showQuickActions = false"
            />
          </v-list>
        </v-card>
      </v-menu>
    </div>

    <!-- Mobile Quick Actions (Alternative) -->
    <v-bottom-sheet v-model="showQuickActions">
      <v-card>
        <v-card-title>Quick Actions</v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="6">
              <v-btn
                block
                color="primary"
                prepend-icon="mdi-plus"
                @click="openTransactionDialog(); showQuickActions = false"
              >
                Add Transaction
              </v-btn>
            </v-col>
              <v-col cols="6">
                <v-btn
                  block
                  color="success"
                  prepend-icon="mdi-target"
                  @click="openGoalDialog(); showQuickActions = false"
                >
                  New Goal
                </v-btn>
              </v-col>
            </v-row>
            <v-row class="mt-2">
              <v-col cols="12">
                <v-btn
                  block
                  color="info"
                  prepend-icon="mdi-chart-line"
                  @click="openBudgetDialog(); showQuickActions = false"
                >
                  New Budget
                </v-btn>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-bottom-sheet>
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useDisplay } from 'vuetify'
import { formatCurrency } from '@/utils/formatters'
import { useDialog } from '@/composables/useDialog'
import { dashboardService } from '@/lib/dashboardService'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import SummaryCards from '@/components/Dashboard/SummaryCards.vue'
import MiniChart from '@/components/Dashboard/MiniChart.vue'
import GoalsList from '@/components/Dashboard/GoalsList.vue'
import BudgetPanel from '@/components/Dashboard/BudgetPanel.vue'
import RecentTransactions from '@/components/Dashboard/RecentTransactions.vue'
import CalendarPanel from '@/components/Dashboard/CalendarPanel.vue'

const { mobile } = useDisplay()
const { openTransactionDialog, openGoalDialog, openBudgetDialog } = useDialog()

// Reactive state
const showQuickActions = ref(false)

// Dashboard stats
const todaySpending = ref(0)
const monthlyBudgetRemaining = ref(0)
const goalsProgressPercentage = ref(0)
const spendingVsBudgetScore = ref(0)

// Computed properties
const isMobile = computed(() => mobile.value)

// Load dashboard stats
const loadDashboardStats = async () => {
  try {
    const stats = await dashboardService.getStats()
    todaySpending.value = stats.todaySpending
    monthlyBudgetRemaining.value = stats.monthlyBudgetRemaining
    goalsProgressPercentage.value = stats.goalsProgress.percentage
    spendingVsBudgetScore.value = stats.spendingVsBudgetScore
  } catch (error) {
    console.error('Failed to load dashboard stats:', error)
    // Keep default values of 0
  }
}

// Helper function to color the budget score
const getBudgetScoreColor = (score: number): string => {
  if (score >= 80) return 'text-success' // Green - excellent
  if (score >= 60) return 'text-info' // Blue - good
  if (score >= 40) return 'text-warning' // Orange - fair
  return 'text-error' // Red - poor
}

// Lifecycle
onMounted(async () => {
  await loadDashboardStats()
})
</script>
