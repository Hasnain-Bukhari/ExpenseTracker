<template>
  <AuthCard
    title="Create Account"
    subtitle="Join us today and start managing your finances"
  >
    <!-- Global Error Alert -->
    <v-alert
      v-if="authStore.error"
      type="error"
      variant="tonal"
      class="mb-6"
      closable
      @click:close="authStore.clearError()"
    >
      {{ authStore.error }}
    </v-alert>

    <!-- Register Form -->
    <v-form ref="formRef" @submit.prevent="handleSubmit" class="register-form">
      <div class="form-fields">
        <!-- Full Name Field -->
        <v-text-field
          v-model="form.name"
          label="Full name"
          type="text"
          variant="outlined"
          prepend-inner-icon="mdi-account-outline"
          :error-messages="nameError"
          :disabled="authStore.isLoading"
          class="mb-4"
          autocomplete="name"
          @blur="validateName"
          @keydown.enter="handleSubmit"
        />

        <!-- Email Field -->
        <v-text-field
          v-model="form.email"
          label="Email address"
          type="email"
          variant="outlined"
          prepend-inner-icon="mdi-email-outline"
          :error-messages="emailError"
          :disabled="authStore.isLoading"
          class="mb-4"
          autocomplete="email"
          @blur="validateEmail"
          @keydown.enter="handleSubmit"
        />

        <!-- Password Field -->
        <PasswordField
          v-model="form.password"
          label="Password"
          prepend-inner-icon="mdi-lock-outline"
          :error-messages="passwordError"
          :disabled="authStore.isLoading"
          :show-strength-meter="true"
          class="mb-4"
          autocomplete="new-password"
          @blur="validatePassword"
          @keydown.enter="handleSubmit"
        />

        <!-- Confirm Password Field -->
        <PasswordField
          v-model="form.confirmPassword"
          label="Confirm password"
          prepend-inner-icon="mdi-lock-check-outline"
          :error-messages="confirmPasswordError"
          :disabled="authStore.isLoading"
          class="mb-4"
          autocomplete="new-password"
          @blur="validateConfirmPassword"
          @keydown.enter="handleSubmit"
        />

        <!-- Terms Checkbox -->
        <v-checkbox
          v-model="form.acceptTerms"
          :error-messages="termsError"
          :disabled="authStore.isLoading"
          class="mb-6 terms-checkbox"
          hide-details="auto"
          @update:model-value="validateTerms"
        >
          <template #label>
            <div class="terms-label text-body-2">
              I agree to the 
              <a href="#" class="text-primary text-decoration-none">Terms of Service</a>
              and 
              <a href="#" class="text-primary text-decoration-none">Privacy Policy</a>
            </div>
          </template>
        </v-checkbox>
      </div>

      <!-- Submit Button -->
      <v-btn
        type="submit"
        color="primary"
        size="large"
        block
        variant="elevated"
        :loading="authStore.isLoading"
        :disabled="!isFormValid || authStore.isLoading"
        class="mb-6 submit-btn"
      >
        Create Account
      </v-btn>

      <!-- Divider -->
      <div class="divider mb-6">
        <v-divider />
        <span class="divider-text text-caption text-medium-emphasis px-3">
          Or sign up with
        </span>
      </div>

      <!-- Social Login Buttons -->
      <div class="social-login-section">
        <div class="social-buttons d-flex flex-column gap-3">
          <SocialLoginButton
            v-for="provider in socialProviders"
            :key="provider.id"
            :provider="provider"
            :loading="socialLoading === provider.id"
            :disabled="authStore.isLoading"
            @click="handleSocialLogin"
          />
        </div>
      </div>
    </v-form>

    <!-- Social Login Modal -->
    <v-dialog
      v-model="showSocialModal"
      max-width="400"
      persistent
    >
      <v-card v-if="selectedProvider" class="social-modal">
        <v-card-title class="text-center pa-6">
          <div class="provider-branding mb-4">
            <v-avatar :color="selectedProvider.color" size="64">
              <v-icon :icon="selectedProvider.icon" size="32" color="white" />
            </v-avatar>
          </div>
          <h3>Sign up with {{ selectedProvider.name }}</h3>
        </v-card-title>

        <v-card-text class="text-center pa-6 pt-0">
          <div class="mock-user-info mb-6">
            <v-avatar size="80" class="mb-3">
              <v-img :src="selectedProvider.mockUser.avatar" />
            </v-avatar>
            <h4 class="text-h6 mb-1">{{ selectedProvider.mockUser.name }}</h4>
            <p class="text-subtitle-2 text-medium-emphasis">
              {{ selectedProvider.mockUser.email }}
            </p>
          </div>

          <v-btn
            color="primary"
            size="large"
            block
            variant="elevated"
            :loading="socialLoading === selectedProvider.id"
            @click="confirmSocialLogin"
            class="mb-3"
          >
            Continue as {{ selectedProvider.mockUser.name }}
          </v-btn>

          <v-btn
            variant="text"
            size="large"
            block
            @click="cancelSocialLogin"
            :disabled="socialLoading === selectedProvider.id"
          >
            Cancel
          </v-btn>
        </v-card-text>
      </v-card>
    </v-dialog>

    <template #footer>
      <div class="footer-links text-center">
        <p class="text-body-2 text-medium-emphasis mb-2">
          Already have an account?
          <router-link to="/auth/login" class="text-primary text-decoration-none font-weight-medium">
            Sign in
          </router-link>
        </p>
      </div>
    </template>
  </AuthCard>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { socialProviders } from '@/data/mockAuth'
import type { SocialProvider } from '@/types/auth'
import AuthCard from '@/components/auth/AuthCard.vue'
import PasswordField from '@/components/auth/PasswordField.vue'
import SocialLoginButton from '@/components/auth/SocialLoginButton.vue'

const router = useRouter()
const authStore = useAuthStore()

// Form state
const formRef = ref()
const form = reactive({
  name: '',
  email: '',
  password: '',
  confirmPassword: '',
  acceptTerms: false
})

// Validation state
const nameError = ref('')
const emailError = ref('')
const passwordError = ref('')
const confirmPasswordError = ref('')
const termsError = ref('')

// Social login state
const showSocialModal = ref(false)
const selectedProvider = ref<SocialProvider | null>(null)
const socialLoading = ref<string | null>(null)

// Form validation
const validateName = () => {
  nameError.value = ''
  if (!form.name) {
    nameError.value = 'Full name is required'
  } else if (form.name.trim().length < 2) {
    nameError.value = 'Name must be at least 2 characters'
  }
}

const validateEmail = () => {
  emailError.value = ''
  if (!form.email) {
    emailError.value = 'Email is required'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    emailError.value = 'Please enter a valid email address'
  }
}

const validatePassword = () => {
  passwordError.value = ''
  if (!form.password) {
    passwordError.value = 'Password is required'
  } else if (form.password.length < 8) {
    passwordError.value = 'Password must be at least 8 characters'
  } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(form.password)) {
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
  } else if (form.password !== form.confirmPassword) {
    confirmPasswordError.value = 'Passwords do not match'
  }
}

const validateTerms = () => {
  termsError.value = ''
  if (!form.acceptTerms) {
    termsError.value = 'You must accept the terms and conditions'
  }
}

const isFormValid = computed(() => {
  return form.name.trim().length >= 2 &&
         form.email && 
         /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email) &&
         form.password &&
         form.password.length >= 8 &&
         /(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(form.password) &&
         form.password === form.confirmPassword &&
         form.acceptTerms &&
         !nameError.value && 
         !emailError.value && 
         !passwordError.value &&
         !confirmPasswordError.value &&
         !termsError.value
})

// Form submission
const handleSubmit = async () => {
  validateName()
  validateEmail()
  validatePassword()
  validateConfirmPassword()
  validateTerms()
  
  if (!isFormValid.value) return
  
  const result = await authStore.register({
    name: form.name.trim(),
    email: form.email,
    password: form.password,
    confirmPassword: form.confirmPassword,
    acceptTerms: form.acceptTerms
  })
  
  if (result.success) {
    // Navigate to onboarding or dashboard
    router.push('/')
  }
}

// Social login handlers
const handleSocialLogin = (provider: SocialProvider) => {
  selectedProvider.value = provider
  showSocialModal.value = true
}

const confirmSocialLogin = async () => {
  if (!selectedProvider.value) return
  
  socialLoading.value = selectedProvider.value.id

  const providerId = selectedProvider.value.id
  const redirectUri = window.location.origin + '/social-callback.html?provider=' + providerId

  // Prefer direct Google or Facebook OAuth from frontend when client id/app id is configured
  let popupUrl: string
  if (providerId === 'google' && import.meta.env.VITE_GOOGLE_CLIENT_ID) {
    const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID
    const scope = encodeURIComponent('openid profile email')
    popupUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientId}&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=token&scope=${scope}&prompt=select_account&include_granted_scopes=true`
  } else if (providerId === 'facebook' && import.meta.env.VITE_FACEBOOK_APP_ID) {
    const appId = import.meta.env.VITE_FACEBOOK_APP_ID
    const scope = encodeURIComponent('email,public_profile')
    popupUrl = `https://www.facebook.com/v16.0/dialog/oauth?client_id=${appId}&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=token&scope=${scope}`
  } else {
    popupUrl = `${import.meta.env.VITE_API_BASE || ''}/auth/oauth/${providerId}?redirect=${encodeURIComponent(redirectUri)}`
  }

  let token: string | null = null

  try {
    const popup = window.open(popupUrl, 'social_auth', 'width=600,height=700')
    if (!popup) {
      // popup blocked; fallback to mock token
      token = `mock-${providerId}:${selectedProvider.value.mockUser.email}`
    } else {
      // wait for message from popup
      token = await new Promise((resolve) => {
        const handler = (e: MessageEvent) => {
          if (e.origin !== window.location.origin) return
          if (!e.data) return
          window.removeEventListener('message', handler)
          resolve(e.data.token ?? null)
        }
        window.addEventListener('message', handler)
        // timeout after 20s
        setTimeout(() => {
          window.removeEventListener('message', handler)
          resolve(null)
        }, 20000)
      })
    }
  } catch (e) {
    token = `mock-${providerId}:${selectedProvider.value.mockUser.email}`
  }

  // call store social login endpoint with token (or undefined to let backend handle full flow)
  const result = await authStore.socialLogin(providerId as 'google' | 'facebook', token ?? undefined)

  socialLoading.value = null
  showSocialModal.value = false

  if (result.success) {
    const redirectTo = router.currentRoute.value.query.redirect as string || '/'
    router.push(redirectTo)
  }
}

const cancelSocialLogin = () => {
  showSocialModal.value = false
  selectedProvider.value = null
  socialLoading.value = null
}
</script>
