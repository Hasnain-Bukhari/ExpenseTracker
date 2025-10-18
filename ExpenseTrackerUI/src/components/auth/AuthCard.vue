<template>
  <div 
    class="auth-card-wrapper"
    v-motion
    :initial="{ opacity: 0, y: 20 }"
    :enter="{ opacity: 1, y: 0, transition: { duration: 500 } }"
  >
    <v-card
      class="auth-card mx-auto"
      :max-width="maxWidth"
      elevation="8"
    >
      <!-- Logo and Title Section -->
      <v-card-text class="text-center pa-6 pa-sm-8">
        <div class="logo-section mb-6">
          <div class="app-logo mb-4">
            <v-avatar size="64" color="primary">
              <v-icon size="32" color="white">mdi-wallet</v-icon>
            </v-avatar>
          </div>
          <h1 class="auth-title text-h4 font-weight-bold mb-2">
            {{ title }}
          </h1>
          <p v-if="subtitle" class="auth-subtitle text-subtitle-1 text-medium-emphasis">
            {{ subtitle }}
          </p>
        </div>
        
        <!-- Form Content Slot -->
        <slot />
      </v-card-text>
      
      <!-- Footer Links Slot -->
      <v-card-actions v-if="$slots.footer" class="justify-center pa-4 pa-sm-6">
        <slot name="footer" />
      </v-card-actions>
    </v-card>
  </div>
</template>

<script setup lang="ts">
interface Props {
  title: string
  subtitle?: string
  maxWidth?: number | string
}

withDefaults(defineProps<Props>(), {
  maxWidth: 440
})
</script>
