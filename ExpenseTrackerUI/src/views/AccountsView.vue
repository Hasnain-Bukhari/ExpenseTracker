<template>
  <v-app>
    <AppHeader />
    <AppNav />
    
    <v-main>
      <v-container fluid class="pa-6">
        <div class="text-center">
          <v-icon
            icon="mdi-bank-outline"
            size="64"
            color="primary"
            class="mb-4"
          />
          <h1 class="text-h3 font-weight-bold text-primary mb-4">
            Accounts
          </h1>
          <p class="text-h6 text-medium-emphasis mb-6">
            Manage your bank accounts, credit cards, and investments.
          </p>
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
</script>
