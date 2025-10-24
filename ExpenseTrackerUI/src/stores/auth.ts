import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, LoginRequest, RegisterRequest, AuthApiResponse } from '@/types/auth'
import api, { setTokens, getRefreshToken, clearTokens } from '@/lib/api'
import { toastService } from '@/services/toastService'

const AUTH_USER_KEY = 'auth_user'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<User | null>(null)
  const token = ref<string | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const userInitials = computed(() => {
    if (!user.value) return ''
    return user.value.name
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2)
  })

  // Actions
  const login = async (credentials: LoginRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const r = await api.post('/auth/login', credentials)
      const response = r.data as AuthApiResponse

      if (response && response.ok && response.user) {
        user.value = response.user as User
        // normalize token field names
        const access = (response as any).accessToken ?? (response as any).token ?? null
        const refresh = (response as any).refreshToken ?? null
        setTokens(access, refresh, !!credentials.remember)
        token.value = access ?? null

        // Persist user for UI persistence
        if (credentials.remember && response.user) {
          localStorage.setItem(AUTH_USER_KEY, JSON.stringify(response.user))
        }

        toastService.loginSuccess()
        return { success: true }
      }

      error.value = (response && (response as any).error) || 'Login failed'
      toastService.loginFailed(error.value)
      return { success: false, error: error.value }
    } catch (err: any) {
      error.value = 'Network error occurred'
      toastService.handleApiError(err, 'Login')
      return { success: false, error: error.value }
    } finally {
      isLoading.value = false
    }
  }

  const register = async (data: RegisterRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const r = await api.post('/auth/register', data)
      const response = r.data as AuthApiResponse

      if (response && response.ok && response.user) {
        user.value = response.user as User
        const access = (response as any).accessToken ?? (response as any).token ?? null
        const refresh = (response as any).refreshToken ?? null
        setTokens(access, refresh, true)
        token.value = access ?? null

        // persist user
        localStorage.setItem(AUTH_USER_KEY, JSON.stringify(response.user))

        toastService.registerSuccess()
        return { success: true }
      }

      error.value = (response && (response as any).error) || 'Registration failed'
      toastService.registerFailed(error.value)
      return { success: false, error: error.value }
    } catch (err: any) {
      error.value = 'Network error occurred'
      toastService.handleApiError(err, 'Registration')
      return { success: false, error: error.value }
    } finally {
      isLoading.value = false
    }
  }

  const socialLogin = async (provider: 'google' | 'facebook', tokenOrMock?: string, codeVerifier?: string) => {
    // tokenOrMock allows passing local mock tokens from UI
    isLoading.value = true
    error.value = null

    try {
      const payload: any = { provider, remember: true }
      if (tokenOrMock) payload.token = tokenOrMock
      if (codeVerifier) payload.codeVerifier = codeVerifier

      const r = await api.post('/auth/social', payload)
      const response = r.data as AuthApiResponse

      if (response && response.ok && response.user) {
        user.value = response.user as User
        const access = (response as any).accessToken ?? (response as any).token ?? null
        const refresh = (response as any).refreshToken ?? null
        setTokens(access, refresh, true)
        token.value = access ?? null
        localStorage.setItem(AUTH_USER_KEY, JSON.stringify(response.user))
        
        toastService.loginSuccess()
        return { success: true }
      }

      error.value = (response && (response as any).error) || 'Social login failed'
      toastService.loginFailed(error.value)
      return { success: false, error: error.value }
    } catch (err: any) {
      error.value = 'Network error occurred'
      toastService.handleApiError(err, 'Social login')
      return { success: false, error: error.value }
    } finally {
      isLoading.value = false
    }
  }

  const logout = () => {
    // attempt server logout if refresh token present
    const refresh = getRefreshToken()
    if (refresh) {
      api.post('/auth/logout', { refreshToken: refresh }).catch(() => {})
    }

    user.value = null
    token.value = null
    error.value = null

    // Clear local persistence
    localStorage.removeItem(AUTH_USER_KEY)
    clearTokens()
    
    toastService.logoutSuccess()
  }

  const loadFromStorage = () => {
    const storedUser = localStorage.getItem(AUTH_USER_KEY)
    // If we have a persisted user, restore it. Tokens are managed by lib/api (refreshToken persisted when remember used)
    if (storedUser) {
      try {
        user.value = JSON.parse(storedUser)
      } catch (err) {
        logout()
      }
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    // State
    user,
    token,
    isLoading,
    error,
    
    // Getters
    isAuthenticated,
    userInitials,
    
    // Actions
    login,
    register,
    socialLogin,
    logout,
    loadFromStorage,
    clearError
  }
})
