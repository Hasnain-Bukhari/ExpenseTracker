<template>
  <v-row class="summary-cards-row" :class="{ 'ma-0': $vuetify.display.mobile }">
    <v-col
      v-for="(card, index) in summaryCards"
      :key="card.title"
      cols="12"
      sm="6"
      lg="3"
      :class="{ 'pa-1': $vuetify.display.mobile, 'pa-2': !$vuetify.display.mobile }"
    >
      <v-card
        :class="[
          'summary-card',
          'dashboard-card',
          card.gradient ? 'gradient-card' : 'glass-card',
          { 'summary-card--loading': isLoading }
        ]"
        v-motion
        :initial="{ opacity: 0, y: 20, scale: 0.9 }"
        :enter="{ 
          opacity: 1, 
          y: 0, 
          scale: 1,
          transition: { 
            delay: index * 100,
            type: 'spring',
            stiffness: 150
          } 
        }"
      >
        <!-- Card Background Pattern -->
        <div class="card-pattern" />
        
        <!-- Loading State -->
        <div v-if="isLoading" class="loading-overlay">
          <div class="skeleton skeleton-card" />
        </div>
        
        <!-- Card Content -->
        <v-card-text 
          v-else 
          class="pa-4 pa-sm-6 d-flex flex-column" 
          :style="{ minHeight: $vuetify.display.mobile ? '120px' : '140px' }"
        >
          <!-- Header Section -->
          <div class="d-flex align-center justify-space-between mb-3 mb-sm-4">
            <div class="card-icon-container">
              <v-avatar
                :size="$vuetify.display.mobile ? 48 : 56"
                :class="[
                  'card-icon',
                  card.gradient ? 'card-icon--inverse' : 'card-icon--primary'
                ]"
              >
                <v-icon
                  :icon="card.icon"
                  :color="card.gradient ? 'white' : 'primary'"
                  :size="$vuetify.display.mobile ? 24 : 28"
                />
              </v-avatar>
            </div>
            
            <!-- Menu Button -->
            <v-menu offset-y>
              <template v-slot:activator="{ props }">
                <v-btn
                  v-bind="props"
                  icon
                  size="small"
                  variant="text"
                  class="card-menu-btn"
                  :color="card.gradient ? 'white' : 'default'"
                >
                  <v-icon size="16">mdi-dots-vertical</v-icon>
                </v-btn>
              </template>
              <v-list density="compact">
                <v-list-item @click="viewDetails(card.key)">
                  <v-list-item-title>View Details</v-list-item-title>
                </v-list-item>
                <v-list-item @click="exportData(card.key)">
                  <v-list-item-title>Export Data</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
          </div>
          
          <!-- Content Section -->
          <div class="flex-grow-1">
            <!-- Title -->
            <h3 
              :class="[
                'card-title text-caption text-sm-body-1 font-weight-medium mb-1 mb-sm-2 text-truncate',
                card.gradient ? 'text-white' : 'text-secondary'
              ]"
            >
              {{ card.title }}
            </h3>
            
            <!-- Main Value -->
            <div class="d-flex align-center mb-2 mb-sm-3 flex-wrap">
              <h2 
                :class="[
                  'card-value text-h5 text-sm-h4 font-weight-bold text-currency mr-2 mr-sm-3',
                  card.gradient ? 'text-white' : 'text-primary'
                ]"
                style="line-height: 1.1;"
              >
                {{ card.value }}
              </h2>
              
              <!-- Value Change Indicator -->
              <v-chip
                v-if="card.change"
                :color="getChangeColor(card.changeType)"
                :variant="card.gradient ? 'flat' : 'tonal'"
                size="small"
                class="change-chip"
              >
                <v-icon
                  :icon="getChangeIcon(card.changeType)"
                  size="12"
                  class="mr-1"
                />
                {{ card.change }}
              </v-chip>
            </div>
            
            <!-- Progress or Mini Chart -->
            <div v-if="card.progress !== undefined" class="mb-2">
              <v-progress-linear
                :model-value="card.progress"
                :color="card.gradient ? 'white' : 'primary'"
                height="4"
                rounded
                class="progress-animated"
              />
              <p 
                :class="[
                  'text-caption mt-1 mb-0',
                  card.gradient ? 'text-white' : 'text-secondary'
                ]"
              >
                {{ card.progressLabel }}
              </p>
            </div>
            
            <!-- Trend Indicator -->
            <div v-if="card.trend" class="trend-container">
              <canvas 
                :ref="`trend-${card.key}`"
                class="trend-chart"
                width="60"
                height="20"
              />
            </div>
          </div>
          
          <!-- Footer Actions -->
          <div class="d-flex justify-space-between align-center">
            <p 
              :class="[
                'text-caption mb-0',
                card.gradient ? 'text-white' : 'text-tertiary'
              ]"
            >
              {{ card.subtitle }}
            </p>
            
            <v-btn
              :color="card.gradient ? 'white' : 'primary'"
              variant="text"
              size="small"
              @click="navigateToDetails(card.key)"
            >
              View More
              <v-icon end size="14">mdi-arrow-right</v-icon>
            </v-btn>
          </div>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useAppStore } from '@/stores'
import { formatCurrency } from '@/utils/formatters'

const router = useRouter()
const store = useAppStore()
const isLoading = ref(false)

// Enhanced summary cards with more data
const summaryCards = computed(() => [
  {
    key: 'balance',
    title: 'Total Balance',
    subtitle: 'Across all accounts',
    value: formatCurrency(store.totalBalance),
    icon: 'mdi-account-balance-wallet',
    gradient: true,
    change: '+2.4%',
    changeType: 'positive' as const,
    progress: 75,
    progressLabel: '75% of monthly goal',
    trend: [2100, 2300, 2800, 3200, 3800, 4100, 4250]
  },
  {
    key: 'spending',
    title: 'Monthly Spend',
    subtitle: 'This month',
    value: formatCurrency(store.monthlySpend),
    icon: 'mdi-credit-card-outline',
    gradient: false,
    change: '+12.5%',
    changeType: 'negative' as const,
    progress: 68,
    progressLabel: '68% of budget used',
    trend: [800, 950, 1200, 1100, 1350, 1400, 1600]
  },
  {
    key: 'income',
    title: 'Monthly Income',
    subtitle: 'All sources',
    value: formatCurrency(store.monthlyIncome),
    icon: 'mdi-trending-up',
    gradient: false,
    change: '+5.2%',
    changeType: 'positive' as const,
    progress: 92,
    progressLabel: 'Above average',
    trend: [5000, 5200, 5400, 5300, 5500, 5600, 5500]
  },
  {
    key: 'savings',
    title: 'Net Savings',
    subtitle: 'This month',
    value: formatCurrency(store.savings),
    icon: 'mdi-piggy-bank',
    gradient: false,
    change: '-3.1%',
    changeType: 'neutral' as const,
    progress: 45,
    progressLabel: 'Below target',
    trend: [3900, 3850, 3800, 3750, 3700, 3650, 3600]
  },
])

// Methods
const getChangeColor = (type: 'positive' | 'negative' | 'neutral'): string => {
  switch (type) {
    case 'positive': return 'success'
    case 'negative': return 'error'
    case 'neutral': return 'warning'
    default: return 'primary'
  }
}

const getChangeIcon = (type: 'positive' | 'negative' | 'neutral'): string => {
  switch (type) {
    case 'positive': return 'mdi-trending-up'
    case 'negative': return 'mdi-trending-down'
    case 'neutral': return 'mdi-minus'
    default: return 'mdi-trending-neutral'
  }
}

const navigateToDetails = (key: string) => {
  // Navigate to detailed view based on card type
  switch (key) {
    case 'balance':
      router.push('/accounts')
      break
    case 'spending':
      router.push('/transactions?filter=expenses')
      break
    case 'income':
      router.push('/transactions?filter=income')
      break
    case 'savings':
      router.push('/goals')
      break
  }
}

const viewDetails = (key: string) => {
  navigateToDetails(key)
}

const exportData = (key: string) => {
  // TODO: Implement data export functionality
  console.log(`Exporting data for ${key}`)
}

// Draw mini trend charts
const drawTrendChart = (canvas: HTMLCanvasElement, data: number[]) => {
  const ctx = canvas.getContext('2d')
  if (!ctx) return

  const width = canvas.width
  const height = canvas.height
  const max = Math.max(...data)
  const min = Math.min(...data)
  const range = max - min || 1

  ctx.clearRect(0, 0, width, height)
  ctx.strokeStyle = 'rgba(var(--v-theme-primary), 0.8)'
  ctx.lineWidth = 1.5
  ctx.lineCap = 'round'
  ctx.lineJoin = 'round'

  ctx.beginPath()
  data.forEach((value, index) => {
    const x = (index / (data.length - 1)) * width
    const y = height - ((value - min) / range) * height
    
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()
}

onMounted(async () => {
  // Simulate loading state
  isLoading.value = true
  setTimeout(() => {
    isLoading.value = false
  }, 500)

  // Draw trend charts after component is mounted
  await nextTick()
  summaryCards.value.forEach((card, index) => {
    if (card.trend) {
      const canvas = document.querySelector(`[ref="trend-${card.key}"]`) as HTMLCanvasElement
      if (canvas) {
        setTimeout(() => drawTrendChart(canvas, card.trend!), index * 100)
      }
    }
  })
})
</script>
