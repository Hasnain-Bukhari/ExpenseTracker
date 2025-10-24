import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import { MotionPlugin } from '@vueuse/motion'

// Vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { mdi } from 'vuetify/iconsets/mdi'
import '@mdi/font/css/materialdesignicons.css'

// Toast notifications
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'

// Global styles
import './styles/global.scss'

import App from './App.vue'

// Determine initial brightness from persisted storage or system preference so Vuetify can be configured correctly
let initialIsDark = false
try {
  const stored = localStorage.getItem('theme:isDark')
  if (stored !== null) initialIsDark = JSON.parse(stored)
  else initialIsDark = typeof window !== 'undefined' && window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
} catch (e) {
  // ignore
}

const vuetify = createVuetify({
  components,
  directives,
  icons: {
    defaultSet: 'mdi',
    sets: {
      mdi,
    },
  },
  theme: {
    defaultTheme: initialIsDark ? 'dark' : 'light',
    themes: {
      light: {
        colors: {
          primary: '#0F766E',
          secondary: '#334155',
          background: '#F8FAFC',
          surface: '#FFFFFF',
          'on-surface': '#1E293B',
          accent: '#F97316',
          success: '#10B981',
          warning: '#F59E0B',
          error: '#EF4444',
          info: '#3B82F6'
        }
      },
      dark: {
        colors: {
          primary: '#80CBC4',
          secondary: '#94A3B8',
          background: '#0F172A',
          surface: '#1E293B',
          'on-surface': '#F1F5F9',
          accent: '#FB923C',
          success: '#10B981',
          warning: '#F59E0B',
          error: '#EF4444',
          info: '#3B82F6'
        }
      }
    }
  },
  defaults: {
    VCard: {
      elevation: 0,
      variant: 'flat',
    },
    VBtn: {
      variant: 'flat',
    },
  },
})

// Toast configuration
const toastOptions = {
  position: 'top-right',
  timeout: 4000,
  closeOnClick: true,
  pauseOnFocusLoss: true,
  pauseOnHover: true,
  draggable: true,
  draggablePercent: 0.6,
  showCloseButtonOnHover: false,
  hideProgressBar: false,
  closeButton: 'button',
  icon: true,
  rtl: false,
  transition: 'Vue-Toastification__bounce',
  maxToasts: 5,
  newestOnTop: true,
  filterBeforeCreate: (toast: any, toasts: any[]) => {
    if (toasts.filter(t => t.type === toast.type).length !== 0) {
      return false
    }
    return toast
  }
}

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(vuetify)
app.use(MotionPlugin)
app.use(Toast, toastOptions)

// Initialize auth store to load persisted state
import { useAuthStore } from '@/stores/auth'
const authStore = useAuthStore()
authStore.loadFromStorage()

// Load theme store to apply persisted theme
import { useThemeStore } from '@/stores/themeStore'
const themeStore = useThemeStore()
themeStore.loadFromStorage()

app.mount('#app')
