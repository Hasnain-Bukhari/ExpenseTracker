import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosRequestConfig } from 'axios'

const REFRESH_KEY = 'auth:refreshToken'

export const api: AxiosInstance = axios.create({
  baseURL: (import.meta.env.VITE_API_BASE as string) || 'http://localhost:5001/v1/'
})

// simple in-memory token container
let accessToken: string | null = null
let refreshToken: string | null = localStorage.getItem(REFRESH_KEY) ?? null

export function getAccessToken() {
  return accessToken    
}

export function getRefreshToken() {
  return refreshToken
}

export function setTokens(access: string | null, refresh?: string | null, remember = false) {
  accessToken = access
  if (typeof refresh !== 'undefined' && refresh !== null) {
    refreshToken = refresh
    if (remember) localStorage.setItem(REFRESH_KEY, refresh)
    else localStorage.removeItem(REFRESH_KEY)
  }
}

export function clearTokens() {
  accessToken = null
  refreshToken = null
  localStorage.removeItem(REFRESH_KEY)
}

// Attach Authorization header if access token present
api.interceptors.request.use((cfg: InternalAxiosRequestConfig) => {
  if (!cfg.headers) cfg.headers = {} as any
  if (accessToken) {
    ;(cfg.headers as any).Authorization = `Bearer ${accessToken}`
  }
  return cfg
})

// create a separate client to call refresh without triggering interceptors
const refreshClient = axios.create({ baseURL: api.defaults.baseURL })

let isRefreshing = false
let failedQueue: Array<{
  resolve: (token?: string | null) => void
  reject: (err: any) => void
  config: AxiosRequestConfig
}> = []

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(p => (error ? p.reject(error) : p.resolve(token)))
  failedQueue = []
}

api.interceptors.response.use(
  r => r,
  async err => {
    const original = err.config as AxiosRequestConfig & { _retry?: boolean }
    if (!original) return Promise.reject(err)

    if (err.response?.status === 401 && !original._retry) {
      original._retry = true

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject, config: original })
        }).then((token: any) => {
          if (token) (original.headers as any).Authorization = `Bearer ${token}`
          return api(original)
        })
      }

      isRefreshing = true
      try {
        const r = await refreshClient.post('/auth/refresh', { refreshToken })
        const data = r.data as any
        const newAccess = data.accessToken ?? data.token ?? null
        const newRefresh = data.refreshToken ?? null
        // persist refresh only if it existed before in storage OR server returned one
        const remember = !!localStorage.getItem(REFRESH_KEY) || !!newRefresh
        setTokens(newAccess, newRefresh, remember)
        processQueue(null, newAccess)
        if (newAccess) (original.headers as any).Authorization = `Bearer ${newAccess}`
        return api(original)
      } catch (e) {
        processQueue(e, null)
        clearTokens()
        return Promise.reject(e)
      } finally {
        isRefreshing = false
      }
    }

    return Promise.reject(err)
  }
)

export default api
