<template>
  <v-app-bar
    :elevation="0"
    color="surface"
    :height="72"
    app
    class="app-header"
  >
    <!-- Navigation Toggle -->
    <v-app-bar-nav-icon
      @click="toggleNav"
      class="nav-toggle ml-2"
      :ripple="false"
      aria-label="Toggle navigation"
      :aria-expanded="!store.sideNavCollapsed"
    >
      <v-icon 
        :icon="store.sideNavCollapsed ? 'mdi-menu' : 'mdi-menu-open'"
        class="nav-icon"
      />
    </v-app-bar-nav-icon>

    <!-- Logo and Brand -->
    <div 
      class="brand-container d-flex align-center ml-2 ml-sm-4"
      v-motion
      :initial="{ opacity: 0, x: -20 }"
      :enter="{ opacity: 1, x: 0, transition: { delay: 200 } }"
    >
      <v-avatar 
        :size="$vuetify.display.mobile ? 36 : 40" 
        class="brand-logo mr-2 mr-sm-3 hover-glow"
      >
        <v-img
          src="/logo.svg"
          alt="ExpenseTracker Logo"
        />
      </v-avatar>
      <div class="brand-text d-none d-sm-block">
        <h1 class="text-subtitle-1 text-sm-h6 font-weight-bold text-primary mb-0 brand-title">
          ExpenseTracker
        </h1>
        <p class="text-caption text-secondary mb-0 brand-subtitle d-none d-md-block">
          Financial Freedom
        </p>
      </div>
    </div>

    <v-spacer />

    <!-- Search Bar (Tablet+) -->
    <div 
      v-if="!$vuetify.display.mobile"
      class="header-search-container mx-3 mx-md-6"
      v-motion
      :initial="{ opacity: 0, scale: 0.8 }"
      :enter="{ opacity: 1, scale: 1, transition: { delay: 400 } }"
    >
      <v-text-field
        v-model="searchQuery"
        prepend-inner-icon="mdi-magnify"
        :placeholder="$vuetify.display.mdAndUp ? 'Search transactions, goals, accounts...' : 'Search...'"
        variant="solo-filled"
        density="compact"
        hide-details
        single-line
        class="header-search"
        :style="{ 
          maxWidth: $vuetify.display.mdAndUp ? '400px' : '200px', 
          minWidth: $vuetify.display.mdAndUp ? '280px' : '160px' 
        }"
        rounded="xl"
      />
    </div>

    <v-spacer />

    <!-- Action Buttons -->
    <div 
      class="header-actions d-flex align-center mr-1 mr-sm-2"
      v-motion
      :initial="{ opacity: 0, x: 20 }"
      :enter="{ opacity: 1, x: 0, transition: { delay: 600 } }"
    >
      <!-- Search Button (Mobile Only) -->
      <v-btn
        v-if="$vuetify.display.mobile"
        icon
        variant="text"
        :size="$vuetify.display.mobile ? 'small' : 'default'"
        class="header-btn mr-1"
        @click="showMobileSearch = !showMobileSearch"
        aria-label="Search"
      >
        <v-icon>mdi-magnify</v-icon>
      </v-btn>

      <!-- Quick Add Button -->
      <v-menu offset-y location="bottom end" :close-on-content-click="false">
        <template v-slot:activator="{ props }">
          <v-btn
            v-bind="props"
            icon
            variant="text"
            :size="$vuetify.display.mobile ? 'small' : 'default'"
            class="header-btn mr-1 mr-sm-2"
            aria-label="Quick add"
          >
            <v-icon color="primary">mdi-plus-circle-outline</v-icon>
            <v-tooltip v-if="!$vuetify.display.mobile" activator="parent" location="bottom">
              Quick Add
            </v-tooltip>
          </v-btn>
        </template>
        
        <v-card elevation="8" rounded="xl" min-width="200">
          <v-list density="compact">
            <v-list-item
              prepend-icon="mdi-plus-circle-outline"
              title="Add Transaction"
              @click="emit('quick-add', 'transaction')"
            />
            <v-list-item
              prepend-icon="mdi-target"
              title="New Goal"
              @click="emit('quick-add', 'goal')"
            />
            <v-list-item
              prepend-icon="mdi-chart-line"
              title="New Budget"
              @click="emit('quick-add', 'budget')"
            />
          </v-list>
        </v-card>
      </v-menu>

      <!-- Notifications -->
      <v-btn
        icon
        variant="text"
        :size="$vuetify.display.mobile ? 'small' : 'default'"
        class="header-btn notifications-btn mr-1 mr-sm-2"
        :class="{ 'has-notifications': notificationCount > 0 }"
        @click="showNotifications = true"
        aria-label="View notifications"
      >
        <v-badge
          :content="notificationCount"
          :model-value="notificationCount > 0"
          color="error"
          :offset-x="$vuetify.display.mobile ? 2 : 4"
          :offset-y="$vuetify.display.mobile ? 2 : 4"
        >
          <v-icon :size="$vuetify.display.mobile ? 20 : 24">mdi-bell-outline</v-icon>
        </v-badge>
        <v-tooltip v-if="!$vuetify.display.mobile" activator="parent" location="bottom">
          Notifications
        </v-tooltip>
      </v-btn>

      <!-- Theme Toggle (Hidden on small mobile) -->
      <v-btn
        v-if="!$vuetify.display.xs"
        icon
        variant="text"
        :size="$vuetify.display.mobile ? 'small' : 'default'"
        class="header-btn mr-1 mr-sm-2"
        @click="toggleTheme"
        aria-label="Toggle theme"
      >
        <v-icon :size="$vuetify.display.mobile ? 20 : 24">{{ isDark ? 'mdi-weather-sunny' : 'mdi-weather-night' }}</v-icon>
        <v-tooltip v-if="!$vuetify.display.mobile" activator="parent" location="bottom">
          {{ isDark ? 'Light Mode' : 'Dark Mode' }}
        </v-tooltip>
      </v-btn>

      <!-- User Profile Menu -->
      <v-menu offset-y min-width="280" class="user-menu">
        <template v-slot:activator="{ props }">
          <v-btn
            v-bind="props"
            class="profile-menu-btn pa-1"
            variant="text"
          >
            <div class="d-flex align-center">
              <v-avatar 
                size="36" 
                class="user-avatar hover-lift"
                :color="authStore.user?.avatar ? 'transparent' : 'primary'"
              >
                <v-img
                  v-if="authStore.user?.avatar"
                  :src="authStore.user.avatar"
                  :alt="authStore.user.name"
                />
                <span v-else class="text-white font-weight-bold">
                  {{ authStore.userInitials }}
                </span>
              </v-avatar>
              <div v-if="!isMobile" class="user-info ml-3 text-left">
                <p class="text-body-2 font-weight-medium mb-0">{{ authStore.user?.name || 'User' }}</p>
                <p class="text-caption text-secondary mb-0">Premium Account</p>
              </div>
            </div>
          </v-btn>
        </template>

        <v-card class="user-menu-card" elevation="8">
          <v-card-text class="pa-4">
            <div class="d-flex align-center mb-3">
              <v-avatar size="48" class="mr-3" :color="authStore.user?.avatar ? 'transparent' : 'primary'">
                <v-img
                  v-if="authStore.user?.avatar"
                  :src="authStore.user.avatar"
                  :alt="authStore.user.name"
                />
                <span v-else class="text-white font-weight-bold text-h6">
                  {{ authStore.userInitials }}
                </span>
              </v-avatar>
              <div>
                <p class="text-subtitle-1 font-weight-medium mb-0">{{ authStore.user?.name || 'User' }}</p>
                <p class="text-body-2 text-secondary mb-0">{{ authStore.user?.email || 'user@example.com' }}</p>
                <v-chip size="x-small" color="primary" variant="tonal" class="mt-1">
                  Premium
                </v-chip>
              </div>
            </div>
          </v-card-text>
          
          <v-divider />
          
          <v-list density="compact" nav>
            <v-list-item 
              prepend-icon="mdi-account-outline"
              title="Profile Settings"
              subtitle="Manage your account"
              @click="navigateToProfile"
            />
            <v-list-item 
              prepend-icon="mdi-chart-box-outline"
              title="Analytics"
              subtitle="View detailed reports"
              @click="navigateToAnalytics"
            />
            <v-list-item 
              prepend-icon="mdi-cog-outline"
              title="Preferences"
              subtitle="App settings"
              @click="navigateToSettings"
            />
            <v-list-item 
              prepend-icon="mdi-help-circle-outline"
              title="Help & Support"
              subtitle="Get assistance"
              @click="showHelp"
            />
          </v-list>
          
          <v-divider />
          
          <v-card-actions class="pa-2">
            <v-btn
              variant="text"
              color="error"
              prepend-icon="mdi-logout"
              block
              @click="signOut"
            >
              Sign Out
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-menu>
    </div>

    <!-- Mobile Search Bar -->
    <v-expand-transition>
      <div v-if="showMobileSearch && $vuetify.display.mobile" class="mobile-search-container">
        <v-text-field
          v-model="searchQuery"
          prepend-inner-icon="mdi-magnify"
          placeholder="Search..."
          variant="solo-filled"
          density="compact"
          hide-details
          single-line
          class="mobile-search-field"
          rounded="xl"
          autofocus
          @keyup.escape="showMobileSearch = false"
        >
          <template v-slot:append-inner>
            <v-btn
              icon
              size="x-small"
              variant="text"
              @click="showMobileSearch = false"
            >
              <v-icon size="16">mdi-close</v-icon>
            </v-btn>
          </template>
        </v-text-field>
      </div>
    </v-expand-transition>
  </v-app-bar>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useDisplay, useTheme } from 'vuetify'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores'
import { useThemeStore } from '@/stores/themeStore'
// ThemeSelector removed; header toggle handles theme switching

const { mobile } = useDisplay()
const theme = useTheme()
const router = useRouter()
const authStore = useAuthStore()
const store = useAppStore()
const themeStore = useThemeStore()

// Define emits
const emit = defineEmits<{
  (e: 'quick-add', action: 'transaction' | 'goal' | 'budget'): void
}>()

// Reactive data
const searchQuery = ref('')
const showNotifications = ref(false)
const showMobileSearch = ref(false)
const notificationCount = ref(3)

// Computed properties
const isMobile = computed(() => mobile.value)
const isDark = computed(() => themeStore.isDark)

// Methods
const toggleNav = () => {
  store.toggleSideNav()
}

const toggleTheme = () => {
  themeStore.toggleDark()
  // keep Vuetify theme in sync if needed
  theme.global.name.value = themeStore.isDark ? 'dark' : 'light'
}

const navigateToProfile = () => {
  router.push('/profile')
}

const navigateToAnalytics = () => {
  router.push('/analytics')
}

const navigateToSettings = () => {
  router.push('/settings')
}

const showHelp = () => {
  // Open help modal or navigate to help page
  console.log('Show help')
}

const signOut = () => {
  authStore.logout()
  router.push('/auth/login')
}
</script>


