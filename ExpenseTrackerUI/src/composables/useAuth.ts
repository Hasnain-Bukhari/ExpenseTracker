import { ref, computed } from 'vue'
import api, { setTokens, getRefreshToken, clearTokens } from '@/lib/api'
import type {
  LoginRequest,
  RegisterRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  AuthApiResponse,
  User
} from '@/types/auth'

const currentUser = ref<User | null>(null)

export function useAuth() {
  const isAuthenticated = computed(() => !!currentUser.value)

  async function register(payload: RegisterRequest) {
    const r = await api.post<AuthApiResponse>('/auth/register', payload)
    const data = r.data
    if (data?.ok && (data.accessToken || (data as any).token)) {
      const access = (data as any).accessToken ?? (data as any).token ?? null
      const refresh = (data as any).refreshToken ?? null
      setTokens(access, refresh, false)
      currentUser.value = data.user ?? null
    }
    return r.data
  }

  async function login(payload: LoginRequest) {
    const r = await api.post<AuthApiResponse>('/auth/login', payload)
    const data = r.data
    if (data?.ok && (data.accessToken || (data as any).token)) {
      const access = (data as any).accessToken ?? (data as any).token ?? null
      const refresh = (data as any).refreshToken ?? null
      setTokens(access, refresh, !!payload.remember)
      currentUser.value = data.user ?? null
    }
    return r.data
  }

  async function socialLogin(provider: 'google' | 'facebook', tokenOrCode?: string, codeVerifier?: string, remember = true) {
    const payload: any = { provider, remember }
    if (tokenOrCode) payload.token = tokenOrCode
    if (codeVerifier) payload.codeVerifier = codeVerifier

    const r = await api.post<AuthApiResponse>('/auth/social', payload)
    const data = r.data

    if (data && data.ok && data.user) {
      currentUser.value = data.user
      const access = (data as any).accessToken ?? (data as any).token ?? null
      const refresh = (data as any).refreshToken ?? null
      setTokens(access, refresh, remember)
      return { success: true, receivedToken: !!tokenOrCode }
    }

    return { success: false, error: (data && (data as any).error) || 'Social login failed' }
  }

  async function logout() {
    const refresh = getRefreshToken()
    try {
      await api.post('/auth/logout', { refreshToken: refresh })
    } catch (e) {
      // ignore errors on logout
    }
    clearTokens()
    currentUser.value = null
  }

  async function forgotPassword(payload: ForgotPasswordRequest) {
    const r = await api.post('/auth/forgot-password', payload)
    return r.data
  }

  async function resetPassword(payload: ResetPasswordRequest) {
    // Backend only expects token and newPassword
    const r = await api.post('/auth/reset-password', {
      token: payload.token,
      newPassword: payload.newPassword
    })
    return r.data
  }

  return { currentUser, isAuthenticated, register, login, socialLogin, logout, forgotPassword, resetPassword }
}
