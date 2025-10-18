# ExpenseTracker UI

A beautiful, modern expense tracking application built with Vue 3, TypeScript, and Vuetify. Track your expenses, set financial goals, and take control of your financial future.

## 🚀 Features

- **Dashboard Overview** - Get a quick snapshot of your financial health
- **Expense Tracking** - Monitor spending across different categories
- **Goal Setting** - Set and track financial goals with progress visualization
- **Budget Management** - Create and manage budgets for better control
- **Visual Reports** - Understand finances with interactive charts
- **Account Management** - Manage multiple accounts in one place
- **Responsive Design** - Works beautifully on desktop, tablet, and mobile

## 🛠️ Tech Stack

- **Vue 3** with Composition API
- **TypeScript** for type safety
- **Vuetify 3** for Material Design components
- **Vite** for fast development and building
- **Pinia** for state management
- **Vue Router** for navigation
- **Chart.js** for data visualization
- **SCSS** for styling

## 📋 Prerequisites

- Node.js >= 16.0.0
- npm or yarn package manager

## 🚀 Quick Start

1. **Install dependencies**
   ```bash
   npm install
   ```

2. **Start development server**
   ```bash
   npm run dev
   ```

3. **Open your browser**
   Navigate to `http://localhost:3000` to see the application.

## 📦 Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint
- `npm run format` - Format code with Prettier

## 🏗️ Project Structure

```
src/
├── components/
│   ├── Layout/
│   │   ├── AppHeader.vue      # Top navigation bar
│   │   ├── AppNav.vue         # Side navigation menu
│   │   └── AppFooter.vue      # Application footer
│   └── Dashboard/
│       ├── SummaryCards.vue   # Financial summary cards
│       ├── MiniChart.vue      # Spending trend chart
│       ├── GoalsList.vue      # Financial goals with progress
│       └── RecentTransactions.vue # Recent transactions table
├── views/
│   ├── HomeView.vue           # Main dashboard page
│   ├── AboutView.vue          # About page
│   ├── AccountsView.vue       # Accounts management
│   ├── GoalsView.vue          # Goals management
│   ├── BudgetsView.vue        # Budget management
│   ├── ReportsView.vue        # Financial reports
│   └── SettingsView.vue       # Application settings
├── stores/
│   └── index.ts               # Pinia store with mock data
├── router/
│   └── index.ts               # Vue Router configuration
├── types/
│   └── index.d.ts             # TypeScript type definitions
├── utils/
│   ├── mockData.ts            # Sample data for development
│   └── formatters.ts          # Utility functions for formatting
├── styles/
│   ├── variables.scss         # SCSS variables and design tokens
│   └── global.scss            # Global styles and utilities
├── App.vue                    # Root component
└── main.ts                    # Application entry point
```

## 🎨 Design System

### Color Palette
- **Primary**: `#2E7D7A` (Teal)
- **Secondary**: `#455A64`
- **Accent**: `#FF7043`
- **Success**: `#4CAF50`
- **Warning**: `#FF9800`
- **Error**: `#F44336`
- **Background**: `#F7FAFC`

### Key Features
- Material Design components via Vuetify
- Responsive layout with mobile-first approach
- Smooth animations and micro-interactions
- Accessible design with proper ARIA attributes
- Consistent spacing and typography

## 📊 Mock Data

The application comes with comprehensive mock data including:
- Sample bank accounts (checking, savings, credit)
- Recent transactions with categories
- Financial goals with progress tracking
- Budget allocations and spending

## 🔧 Customization

### Adding New Routes
1. Create a new view component in `src/views/`
2. Add the route to `src/router/index.ts`
3. Update navigation in `src/components/Layout/AppNav.vue`

### Extending the Store
The Pinia store in `src/stores/index.ts` can be extended with:
- New state properties
- Additional computed getters
- Action methods for API integration

### Styling
- Modify design tokens in `src/styles/variables.scss`
- Add global styles in `src/styles/global.scss`
- Component-specific styles use scoped SCSS

## 🔌 API Integration

The application is structured for easy API integration:

1. **Replace mock data** in `src/utils/mockData.ts` with API calls
2. **Add Axios interceptors** for authentication and error handling
3. **Update store actions** to make HTTP requests
4. **Add loading states** and error handling in components

Example API integration point:
```typescript
// In store actions
const fetchTransactions = async () => {
  try {
    const response = await axios.get('/api/transactions')
    transactions.value = response.data
  } catch (error) {
    console.error('Failed to fetch transactions:', error)
  }
}
```

## 🚀 Deployment

### Build for Production
```bash
npm run build
```

The `dist/` folder will contain the production-ready files.

### Deployment Options
- **Netlify** - Drag and drop the `dist` folder
- **Vercel** - Connect your GitHub repository
- **Firebase Hosting** - Use Firebase CLI
- **Traditional hosting** - Upload `dist` contents to web server

## 🧪 Development Guidelines

### Code Style
- Use TypeScript strict mode
- Follow Vue 3 Composition API patterns
- Use `<script setup>` syntax
- Implement proper type definitions

### Component Structure
```vue
<template>
  <!-- Template content -->
</template>

<script setup lang="ts">
// Imports
// Reactive state
// Computed properties
// Methods
// Lifecycle hooks
</script>

<style lang="scss" scoped>
/* Component styles */
</style>
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [Vue.js](https://vuejs.org/) - The Progressive JavaScript Framework
- [Vuetify](https://vuetifyjs.com/) - Material Design Components
- [Material Design Icons](https://materialdesignicons.com/) - Beautiful icons
- [Chart.js](https://www.chartjs.org/) - Simple yet flexible charting

---

Built with ❤️ using Vue 3 + TypeScript + Vuetify
