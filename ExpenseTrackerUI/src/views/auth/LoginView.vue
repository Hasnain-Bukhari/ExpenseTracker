<template>
  <AuthCard
    title="Welcome Back"
    subtitle="Sign in to your account to continue"
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

    <!-- Login Form -->
    <v-form ref="formRef" @submit.prevent="handleSubmit" class="login-form">
      <div class="form-fields">
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
          class="mb-4"
          autocomplete="current-password"
          @blur="validatePassword"
          @keydown.enter="handleSubmit"
        />

        <!-- Remember Me Checkbox -->
        <div class="form-options d-flex justify-space-between align-center mb-6">
          <v-checkbox
            v-model="form.remember"
            label="Remember me"
            hide-details
            density="compact"
            :disabled="authStore.isLoading"
          />
          <router-link
            to="/auth/forgot-password"
            class="text-primary text-decoration-none text-sm"
          >
            Forgot password?
          </router-link>
        </div>
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
        Sign In
      </v-btn>

      <!-- Divider -->
      <div class="divider mb-6">
        <v-divider />
        <span class="divider-text text-caption text-medium-emphasis px-3">
          Or continue with
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
          <h3>Continue with {{ selectedProvider.name }}</h3>
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
          Don't have an account?
          <router-link to="/auth/register" class="text-primary text-decoration-none font-weight-medium">
            Sign up
          </router-link>
        </p>
      </div>
    </template>
  </AuthCard>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
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
  email: '',
  password: '',
  remember: false
})

// Validation state
const emailError = ref('')
const passwordError = ref('')

// Social login state
const showSocialModal = ref(false)
const selectedProvider = ref<SocialProvider | null>(null)
const socialLoading = ref<string | null>(null)

// Utility functions for PKCE
const base64UrlEncode = (buffer: ArrayBuffer) => {
  const bytes = new Uint8Array(buffer)
  let str = ''
  for (let i = 0; i < bytes.byteLength; i++) {
    str += String.fromCharCode(bytes[i])
  }
  return btoa(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '')
}

const generatePKCE = async () => {
  // create random verifier
  const array = new Uint8Array(64)
  crypto.getRandomValues(array)
  const verifier = Array.from(array).map((b) => ('0' + b.toString(16)).slice(-2)).join('')
  const enc = new TextEncoder().encode(verifier)
  const digest = await crypto.subtle.digest('SHA-256', enc)
  const challenge = base64UrlEncode(digest)
  return { verifier, challenge }
}

// Form validation
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
  }
}

const isFormValid = computed(() => {
  return form.email && 
         form.password && 
         !emailError.value && 
         !passwordError.value &&
         /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)
})

// Form submission
const handleSubmit = async () => {
  validateEmail()
  validatePassword()
  
  if (!isFormValid.value) return
  
  const result = await authStore.login({
    email: form.email,
    password: form.password,
    remember: form.remember
  })
  
  if (result.success) {
    // Redirect to dashboard or intended route
    const redirectTo = router.currentRoute.value.query.redirect as string || '/'
    router.push(redirectTo)
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
  let usedPKCE = false
  let pkceVerifier: string | null = null

  if (providerId === 'google' && import.meta.env.VITE_GOOGLE_CLIENT_ID) {
    // Use Authorization Code + PKCE
    usedPKCE = true
    const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID
    const { verifier, challenge } = await generatePKCE()
    pkceVerifier = verifier
    // store verifier temporarily so it can be retrieved after popup (in case callback handled elsewhere)
    try { sessionStorage.setItem(`pkce_verifier_${providerId}`, verifier) } catch (e) {}
    const scope = encodeURIComponent('openid profile email')
    popupUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientId}&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=code&scope=${scope}&code_challenge=${challenge}&code_challenge_method=S256&prompt=select_account&include_granted_scopes=true`
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
          // allow messages from same origin or wildcard from callback
          if (e.origin !== window.location.origin && e.origin !== 'null') return
          if (!e.data) return
          window.removeEventListener('message', handler)
          resolve(e.data.token ?? null)
        }
        window.addEventListener('MessageEvent' in window ? 'message' : 'message', handler)
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

  // If we used PKCE and received a code, retrieve verifier from sessionStorage
  let codeVerifierToSend: string | undefined = undefined
  if (usedPKCE) {
    try {
      codeVerifierToSend = sessionStorage.getItem(`pkce_verifier_${providerId}`) || undefined
    } catch (e) {
      codeVerifierToSend = undefined
    }
    try { sessionStorage.removeItem(`pkce_verifier_${providerId}`) } catch (e) {}
  }

  // call store social login endpoint. If token is an auth code, include codeVerifier as third arg.
  const result = await authStore.socialLogin(providerId as 'google' | 'facebook', token ?? undefined, codeVerifierToSend)
  
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

// Auto-fill demo credentials on mount
onMounted(() => {
  // Pre-fill with demo credentials for easy testing
  form.email = 'admin@example.com'
  form.password = 'password123'
})
</script>
