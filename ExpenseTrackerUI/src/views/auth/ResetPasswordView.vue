<template>
  <AuthCard
    title="Set New Password"
    subtitle="Create a strong password for your account"
    :max-width="400"
  >
    <!-- Success State -->
    <div v-if="passwordReset" class="success-state text-center">
      <div class="success-icon mb-4">
        <v-avatar size="64" color="success" class="mb-4">
          <v-icon size="32" color="white">mdi-check-circle</v-icon>
        </v-avatar>
      </div>
      
      <h3 class="text-h6 mb-3">Password Reset Successful</h3>
      <p class="text-body-2 text-medium-emphasis mb-6">
        Your password has been successfully updated. You can now sign in with your new password.
      </p>
      
      <v-btn
        color="primary"
        variant="elevated"
        block
        size="large"
        @click="$router.push('/auth/login')"
      >
        Continue to Sign In
      </v-btn>
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

      <!-- Invalid Token Alert -->
      <v-alert
        v-if="!isValidToken"
        type="warning"
        variant="tonal"
        class="mb-6"
      >
        <div class="d-flex flex-column">
          <strong class="mb-2">Invalid or Expired Reset Link</strong>
          <span class="text-body-2">
            This password reset link is invalid or has expired. Please request a new one.
          </span>
          <router-link
            to="/auth/forgot-password"
            class="text-primary text-decoration-none mt-2"
          >
            Request New Reset Link
          </router-link>
        </div>
      </v-alert>

      <!-- Reset Password Form -->
      <v-form 
        v-if="isValidToken"
        ref="formRef" 
        @submit.prevent="handleSubmit" 
        class="reset-password-form"
      >
        <!-- New Password Field -->
        <PasswordField
          v-model="form.newPassword"
          label="New password"
          prepend-inner-icon="mdi-lock-outline"
          :error-messages="passwordError"
          :disabled="isLoading"
          :show-strength-meter="true"
          class="mb-4"
          autocomplete="new-password"
          @blur="validatePassword"
          @keydown.enter="handleSubmit"
        />

        <!-- Confirm Password Field -->
        <PasswordField
          v-model="form.confirmPassword"
          label="Confirm new password"
          prepend-inner-icon="mdi-lock-check-outline"
          :error-messages="confirmPasswordError"
          :disabled="isLoading"
          class="mb-6"
          autocomplete="new-password"
          @blur="validateConfirmPassword"
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
          class="mb-4 submit-btn"
        >
          Update Password
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
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { mockResetPassword } from '@/data/mockAuth'
import AuthCard from '@/components/auth/AuthCard.vue'
import PasswordField from '@/components/auth/PasswordField.vue'

const route = useRoute()
const router = useRouter()

// Form state
const formRef = ref()
const form = reactive({
  newPassword: '',
  confirmPassword: ''
})

// Component state
const isLoading = ref(false)
const passwordReset = ref(false)
const error = ref('')
const passwordError = ref('')
const confirmPasswordError = ref('')
const isValidToken = ref(true)

// Get token from route query
const resetToken = computed(() => route.query.token as string)

// Form validation
const validatePassword = () => {
  passwordError.value = ''
  if (!form.newPassword) {
    passwordError.value = 'Password is required'
  } else if (form.newPassword.length < 8) {
    passwordError.value = 'Password must be at least 8 characters'
  } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(form.newPassword)) {
    passwordError.value = 'Password must contain uppercase, lowercase, and number'
  }
  
  // Revalidate confirm password if it exists
  if (form.confirmPassword) {
    validateConfirmPassword()
  }
}

const validateConfirmPassword = () => {
  confirmPasswordError.value = ''
  if (!form.confirmPassword) {
    confirmPasswordError.value = 'Please confirm your password'
  } else if (form.newPassword !== form.confirmPassword) {
    confirmPasswordError.value = 'Passwords do not match'
  }
}

const isFormValid = computed(() => {
  return form.newPassword &&
         form.newPassword.length >= 8 &&
         /(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(form.newPassword) &&
         form.newPassword === form.confirmPassword &&
         !passwordError.value &&
         !confirmPasswordError.value
})

// Form submission
const handleSubmit = async () => {
  validatePassword()
  validateConfirmPassword()
  
  if (!isFormValid.value) return
  
  isLoading.value = true
  error.value = ''
  
  try {
    const result = await mockResetPassword({
      token: resetToken.value,
      newPassword: form.newPassword,
      confirmPassword: form.confirmPassword
    })
    
    if (result.ok) {
      passwordReset.value = true
    } else {
      error.value = result.error || 'Failed to reset password'
    }
  } catch (err) {
    error.value = 'Network error occurred'
  } finally {
    isLoading.value = false
  }
}

// Validate token on mount
onMounted(() => {
  if (!resetToken.value || !resetToken.value.startsWith('reset_')) {
    isValidToken.value = false
  }
})
</script>
