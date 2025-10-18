<template>
  <v-card class="chart-card dashboard-card">
    <!-- Card Header -->
    <v-card-title class="chart-header pa-4 pa-sm-6 pb-4">
      <div class="header-content w-100">
        <div class="header-main d-flex align-center">
          <v-avatar 
            :size="$vuetify.display.mobile ? 36 : 40" 
            color="primary" 
            variant="tonal"
            class="mr-3"
          >
            <v-icon icon="mdi-chart-line" :size="$vuetify.display.mobile ? 18 : 20" />
          </v-avatar>
          <div class="header-text">
            <h3 class="text-h6 text-sm-h5 font-weight-bold mb-0">Spending Trends</h3>
            <p class="text-caption text-sm-body-2 text-secondary mb-0 d-none d-sm-block">Last 12 months</p>
          </div>
        </div>
        
        <!-- Chart Controls -->
        <div class="chart-controls d-flex align-center mt-3 mt-sm-0">
          <v-btn-toggle
            v-model="selectedPeriod"
            variant="outlined"
            :size="$vuetify.display.mobile ? 'x-small' : 'small'"
            density="compact"
            class="mr-2 mr-sm-3"
          >
            <v-btn value="6m" :size="$vuetify.display.mobile ? 'x-small' : 'small'">6M</v-btn>
            <v-btn value="1y" :size="$vuetify.display.mobile ? 'x-small' : 'small'">1Y</v-btn>
            <v-btn value="all" :size="$vuetify.display.mobile ? 'x-small' : 'small'">All</v-btn>
          </v-btn-toggle>
          
          <v-menu offset-y>
            <template v-slot:activator="{ props }">
              <v-btn
                v-bind="props"
                icon
                :size="$vuetify.display.mobile ? 'x-small' : 'small'"
                variant="text"
              >
                <v-icon :size="$vuetify.display.mobile ? 14 : 16">mdi-dots-vertical</v-icon>
              </v-btn>
            </template>
            <v-list density="compact">
              <v-list-item @click="exportChart">
                <v-list-item-title>Export Chart</v-list-item-title>
              </v-list-item>
              <v-list-item @click="viewFullAnalytics">
                <v-list-item-title>Full Analytics</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
        </div>
      </div>
    </v-card-title>

    <!-- Chart Content -->
    <v-card-text class="pa-0">
      <div class="chart-wrapper pa-3 pa-sm-6 pt-0">
        <!-- Loading State -->
        <div v-if="isLoading" class="chart-loading">
          <div 
            class="d-flex justify-center align-center" 
            :style="{ height: $vuetify.display.mobile ? '200px' : '280px' }"
          >
            <v-progress-circular
              indeterminate
              color="primary"
              :size="$vuetify.display.mobile ? 24 : 32"
            />
          </div>
        </div>
        
        <!-- Chart Container -->
        <div 
          v-else
          class="chart-container"
          v-motion
          :initial="{ opacity: 0, scale: 0.9 }"
          :enter="{ opacity: 1, scale: 1, transition: { delay: 300 } }"
        >
          <canvas 
            ref="chartCanvas" 
            class="main-chart"
            :style="{ maxHeight: $vuetify.display.mobile ? '200px' : '280px' }"
          />
        </div>

        <!-- Chart Legend & Stats -->
        <div class="chart-footer mt-4">
          <v-row>
            <v-col cols="12" md="8">
              <!-- Custom Legend -->
              <div class="chart-legend d-flex flex-wrap gap-4">
                <div 
                  v-for="dataset in chartLegend"
                  :key="dataset.label"
                  class="legend-item d-flex align-center"
                >
                  <div 
                    class="legend-color"
                    :style="{ background: dataset.color }"
                  />
                  <span class="text-body-2 font-weight-medium ml-2">
                    {{ dataset.label }}
                  </span>
                  <span class="text-body-2 text-secondary ml-1">
                    ({{ formatCurrency(dataset.total) }})
                  </span>
                </div>
              </div>
            </v-col>
            
            <v-col cols="12" md="4">
              <!-- Quick Stats -->
              <div class="chart-stats text-right">
                <div class="stat-item mb-1">
                  <span class="text-caption text-secondary">This Month:</span>
                  <span class="text-body-2 font-weight-semibold ml-2">
                    {{ formatCurrency(currentMonthSpending) }}
                  </span>
                </div>
                <div class="stat-item mb-1">
                  <span class="text-caption text-secondary">Average:</span>
                  <span class="text-body-2 font-weight-semibold ml-2">
                    {{ formatCurrency(averageSpending) }}
                  </span>
                </div>
                <div class="stat-item">
                  <span class="text-caption text-secondary">Trend:</span>
                  <v-chip
                    :color="trendColor"
                    size="x-small"
                    variant="tonal"
                    class="ml-2"
                  >
                    <v-icon start size="12">{{ trendIcon }}</v-icon>
                    {{ trendPercentage }}%
                  </v-chip>
                </div>
              </div>
            </v-col>
          </v-row>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
  type ChartData,
  type ChartOptions,
} from 'chart.js'
import { formatCurrency } from '@/utils/formatters'

// Register Chart.js components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

const router = useRouter()
const chartCanvas = ref<HTMLCanvasElement>()
const selectedPeriod = ref('1y')
const isLoading = ref(true)
const chartInstance = ref<ChartJS | null>(null)

// Mock data based on selected period
const chartData = computed((): ChartData<'line'> => {
  const periods = {
    '6m': {
      labels: ['May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'],
      expenses: [1800, 1600, 1750, 1400, 1600, 1850],
      income: [2750, 2650, 2800, 2600, 2700, 2850]
    },
    '1y': {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
      expenses: [1200, 1900, 1300, 1500, 2000, 1800, 1600, 1750, 1400, 1600, 1750, 1900],
      income: [2500, 2800, 2600, 2700, 2900, 2750, 2650, 2800, 2600, 2700, 2800, 2900]
    },
    'all': {
      labels: ['2023 Q1', '2023 Q2', '2023 Q3', '2023 Q4', '2024 Q1', '2024 Q2', '2024 Q3', '2024 Q4'],
      expenses: [4500, 5200, 4800, 5600, 5100, 5800, 5200, 5400],
      income: [8100, 8400, 8200, 8600, 8300, 8700, 8400, 8500]
    }
  }

  const data = periods[selectedPeriod.value as keyof typeof periods]
  
  return {
    labels: data.labels,
    datasets: [
      {
        label: 'Expenses',
        data: data.expenses,
        borderColor: 'rgb(239, 68, 68)',
        backgroundColor: 'rgba(239, 68, 68, 0.1)',
        tension: 0.4,
        fill: true,
        pointBackgroundColor: 'rgb(239, 68, 68)',
        pointBorderColor: 'rgb(var(--v-surface-rgb))',
        pointBorderWidth: 2,
        pointRadius: 4,
        pointHoverRadius: 6,
      },
      {
        label: 'Income',
        data: data.income,
        borderColor: 'rgb(15, 118, 110)',
        backgroundColor: 'rgba(15, 118, 110, 0.1)',
        tension: 0.4,
        fill: true,
        pointBackgroundColor: 'rgb(15, 118, 110)',
        pointBorderColor: 'rgb(var(--v-surface-rgb))',
        pointBorderWidth: 2,
        pointRadius: 4,
        pointHoverRadius: 6,
      },
    ],
  }
})

const chartOptions: ChartOptions<'line'> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false, // We'll use custom legend
    },
    tooltip: {
      backgroundColor: 'rgba(var(--v-surface-rgb), 0.95)',
      titleColor: 'var(--color-text-primary)',
      bodyColor: 'var(--color-text-secondary)',
      borderColor: 'rgba(var(--v-theme-on-surface-rgb), 0.08)',
      borderWidth: 1,
      cornerRadius: 8,
      displayColors: true,
      mode: 'index',
      intersect: false,
      callbacks: {
        label: function(context) {
          return `${context.dataset.label}: ${formatCurrency(context.parsed.y)}`
        }
      }
    },
  },
  scales: {
    x: {
      display: true,
      grid: {
        display: false,
      },
      ticks: {
        color: '#94a3b8',
        font: {
          size: 12,
          weight: 500
        }
      },
    },
    y: {
      display: true,
      grid: {
        color: 'rgba(148, 163, 184, 0.2)',
        drawBorder: false,
      },
      ticks: {
        color: '#94a3b8',
        font: {
          size: 12,
          weight: 500
        },
        callback: function(value) {
          return formatCurrency(value as number)
        },
      },
    },
  },
  interaction: {
    mode: 'nearest',
    axis: 'x',
    intersect: false,
  },
  animation: {
    duration: 1500,
    easing: 'easeInOutQuart',
  },
  hover: {
    animationDuration: 200,
  }
}

// Computed properties for stats
const chartLegend = computed(() => {
  const data = chartData.value
  return data.datasets.map(dataset => ({
    label: dataset.label,
    color: dataset.borderColor as string,
    total: (dataset.data as number[]).reduce((sum, value) => sum + value, 0)
  }))
})

const currentMonthSpending = computed(() => {
  const expenses = chartData.value.datasets[0].data as number[]
  return expenses[expenses.length - 1] || 0
})

const averageSpending = computed(() => {
  const expenses = chartData.value.datasets[0].data as number[]
  return expenses.reduce((sum, value) => sum + value, 0) / expenses.length
})

const trendPercentage = computed(() => {
  const expenses = chartData.value.datasets[0].data as number[]
  if (expenses.length < 2) return 0
  
  const current = expenses[expenses.length - 1]
  const previous = expenses[expenses.length - 2]
  return Math.round(((current - previous) / previous) * 100)
})

const trendColor = computed(() => {
  return trendPercentage.value >= 0 ? 'error' : 'success'
})

const trendIcon = computed(() => {
  return trendPercentage.value >= 0 ? 'mdi-trending-up' : 'mdi-trending-down'
})

// Methods
const createChart = () => {
  if (!chartCanvas.value) return
  
  const ctx = chartCanvas.value.getContext('2d')
  if (!ctx) return

  // Destroy existing chart
  if (chartInstance.value) {
    chartInstance.value.destroy()
  }

  chartInstance.value = new ChartJS(ctx, {
    type: 'line',
    data: chartData.value,
    options: chartOptions,
  })
}

const exportChart = () => {
  if (chartInstance.value) {
    const url = chartInstance.value.toBase64Image()
    const link = document.createElement('a')
    link.download = 'spending-trends.png'
    link.href = url
    link.click()
  }
}

const viewFullAnalytics = () => {
  router.push('/reports')
}

// Watch for period changes
watch(selectedPeriod, () => {
  isLoading.value = true
  setTimeout(() => {
    createChart()
    isLoading.value = false
  }, 500)
})

onMounted(() => {
  setTimeout(() => {
    createChart()
    isLoading.value = false
  }, 1000)
})
</script>
