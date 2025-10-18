import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, LoginRequest, RegisterRequest } from '@/types/auth'
import { mockLogin, mockRegister, mockSocialLogin } from '@/data/mockAuth'

const AUTH_TOKEN_KEY = 'auth_token'
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
      const response = await mockLogin(credentials)
      
      if (response.ok && response.user && response.token) {
        user.value = response.user
        token.value = response.token
        
        // Persist to localStorage if remember me is checked
        if (credentials.remember) {
          localStorage.setItem(AUTH_TOKEN_KEY, response.token)
          localStorage.setItem(AUTH_USER_KEY, JSON.stringify(response.user))
        }
        
        // TODO: Set axios default authorization header
        // axios.defaults.headers.common['Authorization'] = `Bearer ${response.token}`
        
        return { success: true }
      } else {
        error.value = response.error || 'Login failed'
        return { success: false, error: error.value }
      }
    } catch (err) {
      error.value = 'Network error occurred'
      return { success: false, error: error.value }
    } finally {
      isLoading.value = false
    }
  }

  const register = async (data: RegisterRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await mockRegister(data)
      
      if (response.ok && response.user && response.token) {
        user.value = response.user
        token.value = response.token
        
        // Auto-save token after successful registration
        localStorage.setItem(AUTH_TOKEN_KEY, response.token)
        localStorage.setItem(AUTH_USER_KEY, JSON.stringify(response.user))
        
        // TODO: Set axios default authorization header
        // axios.defaults.headers.common['Authorization'] = `Bearer ${response.token}`
        
        return { success: true }
      } else {
        error.value = response.error || 'Registration failed'
        return { success: false, error: error.value }
      }
    } catch (err) {
      error.value = 'Network error occurred'
      return { success: false, error: error.value }
    } finally {
      isLoading.value = false
    }
  }

  const socialLogin = async (provider: 'google' | 'facebook') => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await mockSocialLogin(provider)
      
      if (response.ok && response.user && response.token) {
        user.value = response.user
        token.value = response.token
        
        // Always persist social login
        localStorage.setItem(AUTH_TOKEN_KEY, response.token)
        localStorage.setItem(AUTH_USER_KEY, JSON.stringify(response.user))
        
        // TODO: Set axios default authorization header
        // axios.defaults.headers.common['Authorization'] = `Bearer ${response.token}`
        
        return { success: true }
      } else {
        error.value = response.error || 'Social login failed'
        return { success: false, error: error.value }
      }
    } catch (err) {
      error.value = 'Network error occurred'
      return { success: false, error: error.value }
    } finally {
      isLoading.value = false
    }
  }

  const logout = () => {
    user.value = null
    token.value = null
    error.value = null
    
    // Clear localStorage
    localStorage.removeItem(AUTH_TOKEN_KEY)
    localStorage.removeItem(AUTH_USER_KEY)
    
    // TODO: Clear axios default authorization header
    // delete axios.defaults.headers.common['Authorization']
  }

  const loadFromStorage = () => {
    const storedToken = localStorage.getItem(AUTH_TOKEN_KEY)
    const storedUser = localStorage.getItem(AUTH_USER_KEY)
    
    if (storedToken && storedUser) {
      try {
        token.value = storedToken
        user.value = JSON.parse(storedUser)
        
        // TODO: Set axios default authorization header
        // axios.defaults.headers.common['Authorization'] = `Bearer ${storedToken}`
        
        // TODO: Validate token with backend
        // validateToken(storedToken)
      } catch (err) {
        // Clear invalid data
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
