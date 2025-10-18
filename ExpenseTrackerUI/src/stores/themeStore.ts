import { defineStore } from 'pinia'

const STORAGE_DARK = 'theme:isDark'

function applyThemeVars(dark: boolean) {
  try {
    const root = document.documentElement
    if (dark) {
      root.style.setProperty('--color-background', '#0F172A')
      root.style.setProperty('--color-surface', '#1E293B')
      root.style.setProperty('--color-text-primary', '#F1F5F9')
      root.style.setProperty('--color-text-secondary', '#9FB4C8')
      root.style.setProperty('--color-surface-variant', '#0B1220')

      // border / muted overlays for dark - use rgb vars so rgba(var(--v-theme-on-surface-rgb), a) works
      root.style.setProperty('--v-surface-rgb', '30, 41, 59')
      root.style.setProperty('--v-theme-on-surface-rgb', '241, 245, 249')
      root.style.setProperty('--v-theme-primary-rgb', '128, 203, 196')
      root.style.setProperty('--border-muted', 'rgba(var(--v-theme-on-surface-rgb), 0.06)')
      root.style.setProperty('--panel-overlay', 'rgba(var(--v-theme-on-surface-rgb), 0.04)')
    } else {
      root.style.setProperty('--color-background', '#F8FAFC')
      root.style.setProperty('--color-surface', '#FFFFFF')
      root.style.setProperty('--color-text-primary', '#1E293B')
      root.style.setProperty('--color-text-secondary', '#64748B')
      root.style.setProperty('--color-surface-variant', '#F1F5F9')

      // border / muted overlays for light - reference on-surface rgb
      root.style.setProperty('--v-surface-rgb', '255, 255, 255')
      root.style.setProperty('--v-theme-on-surface-rgb', '30, 41, 59')
      root.style.setProperty('--v-theme-primary-rgb', '15, 118, 110')
      root.style.setProperty('--border-muted', 'rgba(var(--v-theme-on-surface-rgb), 0.06)')
      root.style.setProperty('--panel-overlay', 'rgba(var(--v-theme-on-surface-rgb), 0.04)')
    }
  } catch (e) {
    // ignore
  }
}

export const useThemeStore = defineStore('theme', {
  state: () => ({
    isDark: false as boolean,
  }),
  actions: {
    toggleDark() {
      this.isDark = !this.isDark
      applyThemeVars(this.isDark)
      try {
        localStorage.setItem(STORAGE_DARK, JSON.stringify(this.isDark))
      } catch (e) {
        // ignore
      }
    },
    setDark(value: boolean) {
      this.isDark = value
      applyThemeVars(this.isDark)
      try {
        localStorage.setItem(STORAGE_DARK, JSON.stringify(this.isDark))
      } catch (e) {
        // ignore
      }
    },
    loadFromStorage() {
      try {
        const dark = localStorage.getItem(STORAGE_DARK)
        if (dark !== null) this.isDark = JSON.parse(dark)
        else if (typeof window !== 'undefined' && window.matchMedia) {
          this.isDark = window.matchMedia('(prefers-color-scheme: dark)').matches
        }
        applyThemeVars(this.isDark)
      } catch (e) {
        // ignore
      }
    },
    resetToSystemPreference() {
      try {
        if (typeof window !== 'undefined' && window.matchMedia) {
          this.isDark = window.matchMedia('(prefers-color-scheme: dark)').matches
          applyThemeVars(this.isDark)
          localStorage.setItem(STORAGE_DARK, JSON.stringify(this.isDark))
        }
      } catch (e) {
        // ignore
      }
    }
  }
})
