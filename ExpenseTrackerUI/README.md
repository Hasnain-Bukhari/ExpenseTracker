# Expense Tracker UI

A modern, responsive Vue.js 3 frontend application for the Expense Tracker, built with Vuetify 3 and TypeScript for a superior user experience.

## 🎨 Design Philosophy

The UI follows Material Design principles with a focus on:
- **Accessibility** - WCAG compliant components
- **Responsiveness** - Mobile-first design approach
- **Performance** - Optimized bundle size and lazy loading
- **User Experience** - Intuitive navigation and interactions

## 🛠️ Technology Stack

- **Vue.js 3** - Progressive JavaScript framework with Composition API
- **TypeScript** - Type-safe development
- **Vuetify 3** - Material Design component framework
- **Vite** - Lightning-fast build tool and dev server
- **Pinia** - Modern state management
- **Vue Router** - Client-side routing with guards
- **Axios** - HTTP client with interceptors
- **SCSS** - Enhanced CSS with variables and mixins

## 📁 Project Structure

```
ExpenseTrackerUI/
├── src/
│   ├── components/              # Reusable Vue components
│   │   ├── auth/               # Authentication components
│   │   │   ├── AuthCard.vue    # Login/Register card
│   │   │   ├── PasswordField.vue # Password input with validation
│   │   │   └── SocialLoginButton.vue # Social auth buttons
│   │   ├── Dashboard/          # Dashboard components
│   │   │   ├── AccountSummary.vue
│   │   │   ├── RecentTransactions.vue
│   │   │   ├── QuickActions.vue
│   │   │   └── BudgetOverview.vue
│   │   └── Layout/             # Layout components
│   │       ├── AppHeader.vue   # Top navigation
│   │       ├── AppNav.vue      # Side navigation
│   │       ├── AppFooter.vue   # Footer
│   │       └── AppDrawer.vue    # Mobile drawer
│   ├── views/                  # Page components
│   │   ├── auth/               # Authentication pages
│   │   │   ├── LoginView.vue   # Login page
│   │   │   ├── RegisterView.vue # Registration page
│   │   │   ├── ForgotPasswordView.vue # Password reset
│   │   │   └── ResetPasswordView.vue # Password reset form
│   │   ├── AccountsView.vue    # Account management
│   │   ├── AccountTypesView.vue # Account type management
│   │   ├── CategoriesView.vue  # Category management
│   │   ├── CurrenciesView.vue   # Currency management
│   │   ├── HomeView.vue        # Dashboard home
│   │   ├── BudgetsView.vue     # Budget management
│   │   ├── GoalsView.vue       # Financial goals
│   │   ├── ReportsView.vue     # Reports and analytics
│   │   └── SettingsView.vue    # User settings
│   ├── stores/                 # Pinia state management
│   │   ├── auth.ts            # Authentication state
│   │   ├── themeStore.ts      # Theme management
│   │   └── index.ts           # Store exports
│   ├── types/                  # TypeScript definitions
│   │   ├── auth.ts            # Authentication types
│   │   ├── account.ts         # Account types
│   │   ├── accountType.ts     # Account type types
│   │   ├── category.ts        # Category types
│   │   ├── currency.ts        # Currency types
│   │   └── index.d.ts         # Global type declarations
│   ├── lib/                    # Utilities and services
│   │   └── api.ts             # API client with interceptors
│   ├── composables/            # Vue composables
│   │   └── useAuth.ts         # Authentication composable
│   ├── styles/                 # Global styles
│   │   ├── global.scss        # Global styles
│   │   ├── variables.scss     # SCSS variables
│   │   └── themes.ts          # Theme configuration
│   ├── utils/                  # Utility functions
│   │   ├── formatters.ts      # Data formatting utilities
│   │   ├── mockData.ts        # Mock data for development
│   │   └── themeUtils.ts      # Theme utility functions
│   ├── router/                 # Vue Router configuration
│   │   └── index.ts           # Route definitions and guards
│   ├── App.vue                # Root component
│   └── main.ts                # Application entry point
├── public/                     # Static assets
│   ├── favicon.svg            # Site favicon
│   ├── logo.svg               # Application logo
│   └── social-callback.html   # Social auth callback
├── package.json               # Dependencies and scripts
├── vite.config.ts             # Vite configuration
├── tsconfig.json              # TypeScript configuration
└── README.md                  # This file
```

## 🚀 Getting Started

### Prerequisites
- Node.js 18+
- npm or yarn
- Backend API running (see API README)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ExpenseTracker/ExpenseTrackerUI
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment**
   - Create `.env` file:
   ```env
   VITE_API_BASE=http://localhost:5001/v1/
   VITE_APP_TITLE=Expense Tracker
   ```

4. **Start development server**
   ```bash
   npm run dev
   ```

5. **Access the application**
   - Navigate to `http://localhost:3000`

### Docker Setup (Alternative)

For easier setup and consistent environment:

1. **Using Docker Compose (from project root)**
   ```bash
   cd /path/to/ExpenseTracker
   ./docker-manage.sh start
   ```

2. **Manual Docker build**
   ```bash
   # Build the UI container
   docker build -t expense-tracker-ui .
   
   # Run the container
   docker run -d --name frontend -p 3000:80 expense-tracker-ui
   ```

3. **Access the application**
   - Frontend: `http://localhost:3000`
   - The UI will automatically proxy API calls to the backend

### Environment Variables

**Local Development (.env)**
```env
VITE_API_BASE=http://localhost:5001/v1/
VITE_APP_TITLE=Expense Tracker
```

**Docker Environment (.env.docker)**
```env
VITE_API_BASE=http://api:80/v1/
VITE_APP_TITLE=Expense Tracker
```

## 🎨 UI Components

### Layout Components

**AppHeader**
- Top navigation bar
- User menu and notifications
- Theme toggle button

**AppNav**
- Side navigation drawer
- Collapsible menu items
- Active route highlighting

**AppFooter**
- Footer information
- Links and copyright

### Feature Components

**AuthCard**
- Unified login/register interface
- Form validation
- Social login integration

**AccountSummary**
- Account balance overview
- Quick account actions
- Visual balance indicators

**RecentTransactions**
- Latest transaction list
- Quick transaction actions
- Filtering and sorting

### Form Components

**PasswordField**
- Secure password input
- Strength indicator
- Show/hide toggle

**SocialLoginButton**
- Social authentication buttons
- Loading states
- Error handling

## 🔐 Authentication Flow

### Login Process
1. User enters credentials
2. API validates and returns JWT token
3. Token stored in memory and localStorage
4. User redirected to dashboard
5. Token included in subsequent requests

### Token Management
- **Access Token** - Short-lived (15 minutes)
- **Refresh Token** - Long-lived (7 days)
- **Automatic Refresh** - Seamless token renewal
- **Logout** - Token cleanup and redirect

### Route Guards
- **Authentication Required** - Protected routes
- **Guest Only** - Public routes (login/register)
- **Role-based** - Future admin routes

## 🎯 State Management

### Pinia Stores

**Auth Store**
```typescript
interface AuthState {
  user: User | null
  isAuthenticated: boolean
  isLoading: boolean
  login(credentials: LoginCredentials): Promise<void>
  logout(): void
  refreshToken(): Promise<void>
}
```

**Theme Store**
```typescript
interface ThemeState {
  isDark: boolean
  primaryColor: string
  toggleTheme(): void
  setPrimaryColor(color: string): void
}
```

## 🌐 API Integration

### API Client
- **Base Configuration** - Environment-based URLs
- **Request Interceptors** - Automatic token attachment
- **Response Interceptors** - Token refresh handling
- **Error Handling** - Centralized error management

### API Services
```typescript
// Account management
export const accountApi = {
  list(): Promise<AccountDto[]>
  get(id: string): Promise<AccountDto>
  create(data: CreateAccountDto): Promise<AccountDto>
  update(id: string, data: UpdateAccountDto): Promise<AccountDto>
  delete(id: string): Promise<void>
}

// Similar patterns for other entities
```

## 🎨 Theming System

### Theme Configuration
- **Material Design 3** - Latest design system
- **Custom Colors** - Brand-specific color palette
- **Dark/Light Modes** - User preference support
- **Responsive Breakpoints** - Mobile-first design

### SCSS Variables
```scss
// Color palette
$primary: #1976d2
$secondary: #424242
$accent: #82b1ff
$error: #f44336
$warning: #ff9800
$info: #2196f3
$success: #4caf50

// Typography
$font-family: 'Roboto', sans-serif
$font-size-base: 16px
$line-height-base: 1.5

// Spacing
$spacing-unit: 8px
$border-radius: 4px
```

## 📱 Responsive Design

### Breakpoints
- **Mobile** - < 600px
- **Tablet** - 600px - 960px
- **Desktop** - > 960px

### Mobile Features
- **Touch-friendly** - Large tap targets
- **Swipe Navigation** - Gesture support
- **Collapsible Menu** - Space-efficient navigation
- **Optimized Forms** - Mobile keyboard support

## 🧪 Development

### Available Scripts
```bash
npm run dev          # Start development server
npm run build        # Build for production
npm run preview      # Preview production build
npm run lint         # Run ESLint
npm run type-check   # Run TypeScript checks
```

### Code Style
- **ESLint** - Code quality and consistency
- **Prettier** - Code formatting
- **TypeScript** - Type safety
- **Vue Style Guide** - Vue.js best practices

### Component Guidelines
- **Single Responsibility** - One purpose per component
- **Props Validation** - Type-safe prop definitions
- **Event Handling** - Clear event naming
- **Accessibility** - ARIA labels and keyboard navigation

## 🚀 Build and Deployment

### Production Build
```bash
npm run build
```

### Build Output
- **Optimized Bundle** - Minified and tree-shaken
- **Code Splitting** - Route-based lazy loading
- **Asset Optimization** - Compressed images and fonts
- **Cache Busting** - Versioned asset names

### Deployment Options
- **Static Hosting** - Netlify, Vercel, GitHub Pages
- **CDN** - CloudFlare, AWS CloudFront
- **Server** - Nginx, Apache

## 🔍 Troubleshooting

### Common Issues

**Build Errors:**
- Check Node.js version compatibility
- Clear node_modules and reinstall
- Verify TypeScript configuration

**API Connection Issues:**
- Verify backend is running
- Check CORS configuration
- Validate API base URL

**Authentication Problems:**
- Check token expiration
- Verify JWT secret consistency
- Clear browser storage

**Styling Issues:**
- Verify Vuetify theme configuration
- Check SCSS compilation
- Validate CSS class names

## 📚 Additional Resources

- [Vue.js 3 Documentation](https://vuejs.org/)
- [Vuetify 3 Documentation](https://vuetifyjs.com/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Pinia Documentation](https://pinia.vuejs.org/)
- [Vite Guide](https://vitejs.dev/guide/)

## 🎯 Future Enhancements

### Planned Features
- **PWA Support** - Offline functionality
- **Real-time Updates** - WebSocket integration
- **Advanced Charts** - Financial analytics
- **Mobile App** - React Native or Flutter
- **Internationalization** - Multi-language support

### Performance Optimizations
- **Virtual Scrolling** - Large data sets
- **Image Optimization** - WebP format
- **Bundle Analysis** - Size optimization
- **Caching Strategy** - Service worker

---

**Built with Vue.js 3 and modern web technologies**
