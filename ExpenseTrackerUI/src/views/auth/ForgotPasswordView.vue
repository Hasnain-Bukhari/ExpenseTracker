<template>
  <AuthCard
    title="Reset Password"
    subtitle="Enter your email address and we'll send you a reset link"
    :max-width="400"
  >
    <!-- Success State -->
    <div v-if="emailSent" class="success-state text-center">
      <div class="success-icon mb-4">
        <v-avatar size="64" color="success" class="mb-4">
          <v-icon size="32" color="white">mdi-email-check</v-icon>
        </v-avatar>
      </div>
      
      <h3 class="text-h6 mb-3">Check Your Email</h3>
      <p class="text-body-2 text-medium-emphasis mb-6">
        If an account exists for <strong>{{ form.email }}</strong>, 
        we've sent password reset instructions to your email address.
      </p>
      
      <div class="success-actions">
        <v-btn
          color="primary"
          variant="elevated"
          block
          size="large"
          @click="resetForm"
          class="mb-3"
        >
          Send Another Email
        </v-btn>
        
        <router-link
          to="/auth/login"
          class="text-primary text-decoration-none d-block text-center"
        >
          Back to Sign In
        </router-link>
      </div>
    </div>

    <!-- Form State -->
    <div v-else>
      <!-- Global Error Alert -->
      <v-alert
        v-if="error"
        type="error"
        variant="tonal"
        class="mb-6"
        closable
        @click:close="error = ''"
      >
        {{ error }}
      </v-alert>

      <!-- Forgot Password Form -->
      <v-form ref="formRef" @submit.prevent="handleSubmit" class="forgot-password-form">
        <!-- Email Field -->
        <v-text-field
          v-model="form.email"
          label="Email address"
          type="email"
          variant="outlined"
          prepend-inner-icon="mdi-email-outline"
          :error-messages="emailError"
          :disabled="isLoading"
          class="mb-6"
          autocomplete="email"
          placeholder="Enter your email address"
          @blur="validateEmail"
          @keydown.enter="handleSubmit"
        />

        <!-- Submit Button -->
        <v-btn
          type="submit"
          color="primary"
          size="large"
          block
          variant="elevated"
          :loading="isLoading"
          :disabled="!isFormValid || isLoading"
          class="mb-6 submit-btn"
        >
          Send Reset Instructions
        </v-btn>

        <!-- Back to Login Link -->
        <div class="text-center">
          <router-link
            to="/auth/login"
            class="text-primary text-decoration-none d-inline-flex align-center"
          >
            <v-icon size="18" class="mr-1">mdi-arrow-left</v-icon>
            Back to Sign In
          </router-link>
        </div>
      </v-form>
    </div>
  </AuthCard>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { mockForgotPassword } from '@/data/mockAuth'
import AuthCard from '@/components/auth/AuthCard.vue'

// Form state
const formRef = ref()
const form = reactive({
  email: ''
})

// Component state
const isLoading = ref(false)
const emailSent = ref(false)
const error = ref('')
const emailError = ref('')

// Form validation
const validateEmail = () => {
  emailError.value = ''
  if (!form.email) {
    emailError.value = 'Email is required'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    emailError.value = 'Please enter a valid email address'
  }
}

const isFormValid = computed(() => {
  return form.email && 
         /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email) &&
         !emailError.value
})

// Form submission
const handleSubmit = async () => {
  validateEmail()
  
  if (!isFormValid.value) return
  
  isLoading.value = true
  error.value = ''
  
  try {
    const result = await mockForgotPassword({ email: form.email })
    
    if (result.ok) {
      emailSent.value = true
    } else {
      error.value = result.error || 'Failed to send reset email'
    }
  } catch (err) {
    error.value = 'Network error occurred'
  } finally {
    isLoading.value = false
  }
}

// Reset form to initial state
const resetForm = () => {
  emailSent.value = false
  form.email = ''
  emailError.value = ''
  error.value = ''
}
</script>
