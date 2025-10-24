<template>
  <v-navigation-drawer
    v-model="drawer"
    :rail="isCollapsed && !$vuetify.display.mobile"
    :temporary="$vuetify.display.mobile"
    :permanent="!$vuetify.display.mobile"
    color="surface"
    :width="$vuetify.display.mobile ? 280 : 280"
    :rail-width="72"
    class="app-nav"
  >
    <!-- Navigation Header -->
    <div class="nav-header" :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }">
      <div 
        class="d-flex align-center"
        v-motion
        :initial="{ opacity: 0, y: -10 }"
        :enter="{ opacity: 1, y: 0, transition: { delay: 100 } }"
      >
        <v-avatar :size="$vuetify.display.mobile ? 28 : 32" class="nav-brand-logo mr-3">
          <span class="text-white font-weight-bold text-caption">ET</span>
        </v-avatar>
        <div class="nav-brand-text" :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }">
          <p class="text-subtitle-2 text-sm-subtitle-1 font-weight-semibold mb-0">Navigation</p>
          <p class="text-caption text-secondary mb-0 d-none d-sm-block">Quick Access</p>
        </div>
      </div>
    </div>

    <v-divider v-if="!isCollapsed" class="mx-3" />

    <!-- Main Navigation -->
    <v-list 
      density="compact" 
      nav 
      class="nav-list"
    >
      <!-- Primary Navigation Items -->
      <div class="nav-section">
        <v-list-subheader 
          class="nav-section-title"
          :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }"
        >
          MAIN
        </v-list-subheader>
        
        <v-list-item
          v-for="(item, index) in primaryNavItems"
          :key="item.to"
          :to="item.to"
          :prepend-icon="item.icon"
          :title="item.title"
          :active="isActive(item.to)"
          class="nav-item mb-1"
          :class="{ active: isActive(item.to) }"
          rounded="xl"
          :aria-label="item.title"
          v-motion
          :initial="{ opacity: 0, x: -20 }"
          :enter="{ 
            opacity: 1, 
            x: 0, 
            transition: { 
              delay: 200 + (index * 50),
              type: 'spring',
              stiffness: 200
            } 
          }"
        >
          <template v-slot:prepend>
            <v-icon 
              :icon="item.icon" 
              class="nav-item-icon"
            />
          </template>
          
          <template v-slot:append v-if="item.badge && !isCollapsed">
            <v-chip
              size="x-small"
              :color="item.badgeColor || 'primary'"
              variant="tonal"
              class="nav-badge"
              :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }"
            >
              {{ item.badge }}
            </v-chip>
          </template>
          
          <template v-slot:title>
            <span class="nav-item-text" :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }">
              {{ item.title }}
            </span>
          </template>
          
          <v-tooltip
            v-if="isCollapsed"
            activator="parent"
            location="end"
            :text="item.title"
          />
        </v-list-item>
      </div>

      <v-divider class="my-3 mx-2" />

      <!-- Categories Navigation Items (new) -->
      <div class="nav-section">
        <v-list-subheader 
          class="nav-section-title"
          :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }"
        >
          LOOKUPS - CATEGORIES
        </v-list-subheader>

        <v-list-item
          :to="'/categories'"
          prepend-icon="mdi-tag-multiple"
          title="Categories"
          :active="isActive('/categories')"
          class="nav-item mb-1"
          :class="{ active: isActive('/categories') }"
          rounded="xl"
          aria-label="Categories"
          v-motion
          :initial="{ opacity: 0, x: -20 }"
          :enter="{ opacity: 1, x: 0, transition: { delay: 350, type: 'spring', stiffness: 200 } }"
        >
          <template v-slot:prepend>
            <v-icon 
              icon="mdi-tag-multiple"
              class="nav-item-icon"
            />
          </template>

          <template v-slot:title>
            <span class="nav-item-text" :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }">
              Categories
            </span>
          </template>

          <v-tooltip
            v-if="isCollapsed"
            activator="parent"
            location="end"
            :text="'Categories'"
          />
        </v-list-item>

        <v-list-item
          :to="'/category-types'"
          prepend-icon="mdi-tag-multiple-outline"
          title="Category Types"
          :active="isActive('/category-types')"
          class="nav-item mb-1"
          :class="{ active: isActive('/category-types') }"
          rounded="xl"
          aria-label="Category Types"
          v-motion
          :initial="{ opacity: 0, x: -20 }"
          :enter="{ opacity: 1, x: 0, transition: { delay: 400, type: 'spring', stiffness: 200 } }"
        >
          <template v-slot:prepend>
            <v-icon 
              icon="mdi-tag-multiple-outline"
              class="nav-item-icon"
            />
          </template>

          <template v-slot:title>
            <span class="nav-item-text" :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }">
              Category Types
            </span>
          </template>

          <v-tooltip
            v-if="isCollapsed"
            activator="parent"
            location="end"
            :text="'Category Types'"
          />
        </v-list-item>

        <v-list-item
          :to="'/account-types'"
          prepend-icon="mdi-bank"
          title="Account Types"
          :active="isActive('/account-types')"
          class="nav-item mb-1"
          :class="{ active: isActive('/account-types') }"
          rounded="xl"
          aria-label="Account Types"
          v-motion
          :initial="{ opacity: 0, x: -20 }"
          :enter="{ opacity: 1, x: 0, transition: { delay: 450, type: 'spring', stiffness: 200 } }"
        >
          <template v-slot:prepend>
            <v-icon 
              icon="mdi-bank"
              class="nav-item-icon"
            />
          </template>

          <template v-slot:title>
            <span class="nav-item-text" :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }">
              Account Types
            </span>
          </template>

          <v-tooltip
            v-if="isCollapsed"
            activator="parent"
            location="end"
            :text="'Account Types'"
          />
        </v-list-item>

        <v-list-item
          :to="'/currencies'"
          prepend-icon="mdi-currency-usd"
          title="Currencies"
          :active="isActive('/currencies')"
          class="nav-item mb-1"
          :class="{ active: isActive('/currencies') }"
          rounded="xl"
          aria-label="Currencies"
          v-motion
          :initial="{ opacity: 0, x: -20 }"
          :enter="{ opacity: 1, x: 0, transition: { delay: 500, type: 'spring', stiffness: 200 } }"
        >
          <template v-slot:prepend>
            <v-icon 
              icon="mdi-currency-usd"
              class="nav-item-icon"
            />
          </template>

          <template v-slot:title>
            <span class="nav-item-text" :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }">
              Currencies
            </span>
          </template>

          <v-tooltip
            v-if="isCollapsed"
            activator="parent"
            location="end"
            :text="'Currencies'"
          />
        </v-list-item>
      </div>

      <v-divider class="my-3 mx-2" />

      <!-- Secondary Navigation Items -->
      <div class="nav-section">
        <v-list-subheader 
          class="nav-section-title"
          :class="{ collapsed: isCollapsed && !$vuetify.display.mobile }"
        >
          TOOLS
        </v-list-subheader>
        
        <v-list-item
          v-for="(item, index) in secondaryNavItems"
          :key="item.to"
          :to="item.to"
          :prepend-icon="item.icon"
          :title="item.title"
          :active="isActive(item.to)"
          class="nav-item mb-1"
          :class="{ active: isActive(item.to) }"
          rounded="xl"
          :aria-label="item.title"
          v-motion
          :initial="{ opacity: 0, x: -20 }"
          :enter="{ 
            opacity: 1, 
            x: 0, 
            transition: { 
              delay: 400 + (index * 50),
              type: 'spring',
              stiffness: 200
            } 
          }"
        >
          <template v-slot:prepend>
            <v-icon 
              :icon="item.icon" 
              class="nav-icon"
              :class="{ 'nav-icon--active': isActive(item.to) }"
            />
          </template>
          
          <v-tooltip
            v-if="isCollapsed"
            activator="parent"
            location="end"
            :text="item.title"
          />
        </v-list-item>
      </div>
    </v-list>

    <!-- Collapse Toggle Button -->
    <template v-slot:append>
      <div class="nav-footer pa-3">
        <v-btn
          :icon="isCollapsed ? 'mdi-chevron-right' : 'mdi-chevron-left'"
          size="small"
          @click="toggleCollapse"
          variant="text"
          class="collapse-btn"
          :aria-label="isCollapsed ? 'Expand navigation' : 'Collapse navigation'"
          :aria-expanded="!isCollapsed"
        >
          <v-icon class="collapse-icon" />
        </v-btn>
        
        <div v-if="!isCollapsed" class="collapse-hint mt-2">
          <p class="text-caption text-center text-secondary mb-0">
            Click to {{ isCollapsed ? 'expand' : 'collapse' }}
          </p>
        </div>
      </div>
    </template>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useDisplay } from 'vuetify'
import { useAppStore } from '@/stores'
import type { NavItem } from '@/types'

const route = useRoute()
const store = useAppStore()
const { mobile } = useDisplay()

// Computed properties
const drawer = computed({
  get: () => mobile.value ? !store.sideNavCollapsed : true,
  set: (value: boolean) => {
    if (mobile.value && !value) {
      // Close drawer on mobile when clicked outside
      if (!store.sideNavCollapsed) {
        store.toggleSideNav()
      }
    }
  }
})

const isCollapsed = computed(() => 
  mobile.value ? false : store.sideNavCollapsed
)

// Auto-close drawer on mobile when route changes
watch(route, () => {
  if (mobile.value && !store.sideNavCollapsed) {
    store.toggleSideNav()
  }
})

// Navigation items
const primaryNavItems: (NavItem & { badge?: string; badgeColor?: string })[] = [
  {
    title: 'Dashboard',
    icon: 'mdi-view-dashboard-outline',
    to: '/',
    badge: 'New',
    badgeColor: 'success'
  },
  {
    title: 'Accounts',
    icon: 'mdi-bank-outline',
    to: '/accounts',
  },
  {
    title: 'Transactions',
    icon: 'mdi-swap-horizontal',
    to: '/transactions',
  },
  {
    title: 'Goals',
    icon: 'mdi-target',
    to: '/goals',
    badge: '3',
    badgeColor: 'primary'
  },
  {
    title: 'Budgets',
    icon: 'mdi-calculator-variant-outline',
    to: '/budgets',
  },
]

const secondaryNavItems: NavItem[] = [
  {
    title: 'Reports',
    icon: 'mdi-chart-line',
    to: '/reports',
  },
  {
    title: 'Settings',
    icon: 'mdi-cog-outline',
    to: '/settings',
  },
  {
    title: 'About',
    icon: 'mdi-information-outline',
    to: '/about',
  },
]

// Methods
const toggleCollapse = () => {
  store.toggleSideNav()
}

const isActive = (path: string): boolean => {
  if (path === '/') {
    return route.path === '/'
  }
  return route.path.startsWith(path)
}
</script>
