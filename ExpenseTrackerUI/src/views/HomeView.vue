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
                @click="showAddTransaction = true"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-plus</v-icon>
                Add Transaction
              </v-btn>
              
              <v-btn
                color="success"
                variant="tonal"
                :prepend-icon="$vuetify.display.mobile ? undefined : 'mdi-target'"
                :size="$vuetify.display.mobile ? 'small' : 'default'"
                @click="showAddGoal = true"
              >
                <v-icon v-if="$vuetify.display.mobile" class="mr-2">mdi-target</v-icon>
                New Goal
              </v-btn>
            </div>
          </div>
          
          <!-- Quick Stats Bar -->
          <div class="quick-stats-bar mt-6">
            <v-row>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Today's Spending</p>
                  <p class="text-h6 font-weight-bold text-primary mb-0">{{ formatCurrency(247.32) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Weekly Budget Left</p>
                  <p class="text-h6 font-weight-bold text-success mb-0">{{ formatCurrency(1652.68) }}</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Goals Progress</p>
                  <p class="text-h6 font-weight-bold text-warning mb-0">73%</p>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="quick-stat text-center">
                  <p class="text-caption text-secondary mb-1">Credit Score</p>
                  <p class="text-h6 font-weight-bold text-info mb-0">742</p>
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
            <v-col cols="12" lg="8">
              <MiniChart />
            </v-col>
            <v-col cols="12" lg="4">
              <GoalsList />
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

    <!-- Quick Action Dialogs -->
    <v-dialog v-model="showAddTransaction" max-width="600">
      <v-card>
        <v-card-title>Add New Transaction</v-card-title>
        <v-card-text>
          <p>Transaction form would go here...</p>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="showAddTransaction = false">Cancel</v-btn>
          <v-btn color="primary" @click="showAddTransaction = false">Add</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="showAddGoal" max-width="600">
      <v-card>
        <v-card-title>Create New Goal</v-card-title>
        <v-card-text>
          <p>Goal creation form would go here...</p>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="showAddGoal = false">Cancel</v-btn>
          <v-btn color="primary" @click="showAddGoal = false">Create</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

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
              prepend-icon="mdi-plus"
              title="Add Transaction"
              @click="showAddTransaction = true; showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-target"
              title="New Goal"
              @click="showAddGoal = true; showQuickActions = false"
            />
            <v-list-item
              prepend-icon="mdi-chart-line"
              title="View Reports"
              @click="$router.push('/reports'); showQuickActions = false"
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
                @click="showAddTransaction = true; showQuickActions = false"
              >
                Add Transaction
              </v-btn>
            </v-col>
            <v-col cols="6">
              <v-btn
                block
                color="success"
                prepend-icon="mdi-target"
                @click="showAddGoal = true; showQuickActions = false"
              >
                New Goal
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-bottom-sheet>
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useDisplay } from 'vuetify'
import { useRouter } from 'vue-router'
import { formatCurrency } from '@/utils/formatters'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import SummaryCards from '@/components/Dashboard/SummaryCards.vue'
import MiniChart from '@/components/Dashboard/MiniChart.vue'
import GoalsList from '@/components/Dashboard/GoalsList.vue'
import RecentTransactions from '@/components/Dashboard/RecentTransactions.vue'

const { mobile } = useDisplay()
const router = useRouter()

// Reactive state
const showAddTransaction = ref(false)
const showAddGoal = ref(false)
const showQuickActions = ref(false)

// Computed properties
const isMobile = computed(() => mobile.value)
</script>
