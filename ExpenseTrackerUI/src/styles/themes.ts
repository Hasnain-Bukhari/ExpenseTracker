export type ThemeKey = 'default' | 'midnight' | 'solar' | 'high-contrast'

export interface ThemeDefinition {
  primary: string
  secondary: string
  surface: string
  background: string
  text: string
  cardBg: string
  cardGlass: string
  muted: string
  accent: string
  chart1: string
  chart2: string
  shadow1: string
  radiusLg: string
  transitionFast: string
}

export const themes: Record<ThemeKey, { light: ThemeDefinition; dark: ThemeDefinition }> = {
  default: {
    light: {
      primary: '#00796B',
      secondary: '#334155',
      surface: '#FFFFFF',
      background: '#F8FAFC',
      text: '#1E293B',
      // prefer theme-aware surface tokens; runtime will compute appropriate rgba
      cardBg: 'rgba(var(--v-surface-rgb), 0.95)',
      cardGlass: 'rgba(var(--v-surface-rgb), 0.75)',
      muted: '#64748B',
      accent: '#F97316',
      chart1: '#14B8A6',
      chart2: '#5EEAD4',
      shadow1: '0 6px 18px rgba(16,24,40,0.08)',
      radiusLg: '12px',
      transitionFast: '150ms ease-in-out',
    },
    dark: {
      primary: '#80CBC4',
      secondary: '#94A3B8',
      surface: '#1E293B',
      background: '#0F172A',
      text: '#F1F5F9',
      cardBg: 'rgba(30,41,59,0.85)',
      cardGlass: 'rgba(14,22,34,0.6)',
      muted: '#94A3B8',
      accent: '#FB923C',
      chart1: '#14B8A6',
      chart2: '#5EEAD4',
      shadow1: '0 8px 24px rgba(2,6,23,0.6)',
      radiusLg: '12px',
      transitionFast: '150ms ease-in-out',
    }
  },

  midnight: {
    light: {
      primary: '#334155',
      secondary: '#475569',
      surface: '#FFFFFF',
      background: '#F1F5F9',
      text: '#0F172A',
      cardBg: 'rgba(var(--v-surface-rgb), 0.96)',
      cardGlass: 'rgba(var(--v-surface-rgb), 0.8)',
      muted: '#64748B',
      accent: '#06B6D4',
      chart1: '#0ea5a8',
      chart2: '#06b6d4',
      shadow1: '0 8px 20px rgba(3,7,18,0.06)',
      radiusLg: '12px',
      transitionFast: '150ms ease-in-out',
    },
    dark: {
      primary: '#0f172a',
      secondary: '#0b1220',
      surface: '#071028',
      background: '#010617',
      text: '#E6EEF6',
      cardBg: 'rgba(10,18,32,0.8)',
      cardGlass: 'rgba(8,12,24,0.6)',
      muted: '#9FB4C8',
      accent: '#06B6D4',
      chart1: '#06b6d4',
      chart2: '#0ea5a8',
      shadow1: '0 12px 36px rgba(3,7,18,0.6)',
      radiusLg: '12px',
      transitionFast: '150ms ease-in-out',
    }
  },

  solar: {
    light: {
      primary: '#FF7043',
      secondary: '#7C3AED',
      surface: '#FFFDF9',
      background: '#FFF8F1',
      text: '#2B2B2B',
      cardBg: 'rgba(var(--v-surface-rgb), 0.98)',
      cardGlass: 'rgba(var(--v-surface-rgb), 0.85)',
      muted: '#6B7280',
      accent: '#FFB86B',
      chart1: '#FF7043',
      chart2: '#F59E0B',
      shadow1: '0 8px 24px rgba(43,43,43,0.06)',
      radiusLg: '12px',
      transitionFast: '150ms ease-in-out',
    },
    dark: {
      primary: '#FF8A65',
      secondary: '#B794F4',
      surface: '#1F2937',
      background: '#0B0A0A',
      text: '#F7F3EE',
      cardBg: 'rgba(28,30,32,0.85)',
      cardGlass: 'rgba(28,30,32,0.6)',
      muted: '#B0B0B0',
      accent: '#FFB86B',
      chart1: '#FF8A65',
      chart2: '#FFB86B',
      shadow1: '0 12px 36px rgba(0,0,0,0.6)',
      radiusLg: '12px',
      transitionFast: '150ms ease-in-out',
    }
  },

  'high-contrast': {
    light: {
      primary: '#FFD400',
      secondary: '#000000',
      surface: '#FFFFFF',
      background: '#FFFFFF',
      text: '#000000',
      cardBg: 'rgba(var(--v-surface-rgb), 1)',
      cardGlass: 'rgba(var(--v-surface-rgb), 1)',
      muted: '#000000',
      accent: '#FFD400',
      chart1: '#FFD400',
      chart2: '#000000',
      shadow1: 'none',
      radiusLg: '6px',
      transitionFast: '100ms linear',
    },
    dark: {
      primary: '#FFD400',
      secondary: '#000000',
      surface: '#000000',
      background: '#000000',
      text: '#FFFFFF',
      cardBg: 'rgba(var(--v-surface-rgb), 1)',
      cardGlass: 'rgba(var(--v-surface-rgb), 1)',
      muted: '#FFFFFF',
      accent: '#FFD400',
      chart1: '#FFD400',
      chart2: '#FFFFFF',
      shadow1: 'none',
      radiusLg: '6px',
      transitionFast: '100ms linear',
    }
  }
}

export const themeLabels: Record<ThemeKey, { name: string; icon?: string }> = {
  default: { name: 'Default' },
  midnight: { name: 'Midnight' },
  solar: { name: 'Solar' },
  'high-contrast': { name: 'High Contrast' },
}
