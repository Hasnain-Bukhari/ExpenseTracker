<template>
  <v-app>
    <AppHeader />
    <AppNav />
    
    <v-main>
      <v-container fluid class="pa-6">
        <div class="text-center">
          <v-icon
            icon="mdi-information-outline"
            size="64"
            color="primary"
            class="mb-4"
          />
          <h1 class="text-h3 font-weight-bold text-primary mb-4">
            About ExpenseTracker
          </h1>
          <p class="text-h6 text-medium-emphasis mb-6 mx-auto" style="max-width: 600px;">
            Your personal finance companion built with Vue 3, TypeScript, and Vuetify.
            Track expenses, set goals, and take control of your financial future.
          </p>
          
          <v-row justify="center" class="mb-6">
            <v-col cols="12" md="8" lg="6">
              <v-card class="dashboard-card pa-6">
                <h2 class="text-h5 font-weight-bold mb-4">Features</h2>
                <v-list>
                  <v-list-item
                    v-for="feature in features"
                    :key="feature.title"
                    :prepend-icon="feature.icon"
                  >
                    <v-list-item-title class="font-weight-medium">
                      {{ feature.title }}
                    </v-list-item-title>
                    <v-list-item-subtitle>
                      {{ feature.description }}
                    </v-list-item-subtitle>
                  </v-list-item>
                </v-list>
              </v-card>
            </v-col>
          </v-row>

          <v-btn
            color="primary"
            size="large"
            to="/"
            prepend-icon="mdi-arrow-left"
          >
            Back to Dashboard
          </v-btn>
        </div>
      </v-container>
    </v-main>

    <AppFooter />
  </v-app>
</template>

<script setup lang="ts">
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'

const features = [
  {
    title: 'Dashboard Overview',
    description: 'Get a quick snapshot of your financial health',
    icon: 'mdi-view-dashboard-outline',
  },
  {
    title: 'Expense Tracking',
    description: 'Monitor your spending across different categories',
    icon: 'mdi-credit-card-outline',
  },
  {
    title: 'Goal Setting',
    description: 'Set and track your financial goals',
    icon: 'mdi-target',
  },
  {
    title: 'Budget Management',
    description: 'Create and manage budgets for better control',
    icon: 'mdi-calculator-variant-outline',
  },
  {
    title: 'Visual Reports',
    description: 'Understand your finances with charts and graphs',
    icon: 'mdi-chart-line',
  },
  {
    title: 'Account Management',
    description: 'Manage multiple accounts in one place',
    icon: 'mdi-bank-outline',
  },
]
</script>
