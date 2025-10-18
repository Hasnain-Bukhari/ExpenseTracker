# Authentication System Testing Guide

## Quick Setup and Testing

### 1. Start the Development Server
```bash
npm install
npm run dev
```

### 2. Test Authentication Flow

#### Login Testing
- Navigate to `/auth/login`
- Use demo credentials:
  - **Email**: `admin@example.com`
  - **Password**: `password123`
- Or use: `user@example.com` / `password123`
- Try "Remember me" checkbox to test persistence

#### Registration Testing
- Navigate to `/auth/register`
- Fill out the form with:
  - **Full Name**: Any name (min 2 chars)
  - **Email**: Any valid email (not existing ones)
  - **Password**: Must be 8+ chars with uppercase, lowercase, and number
  - **Confirm Password**: Must match
  - **Accept Terms**: Must be checked
- Watch the password strength meter in real-time
- Try duplicate email (e.g., `admin@example.com`) to see conflict error

#### Social Login Testing
- Click "Continue with Google" or "Continue with Facebook"
- Mock social modal will appear with fake user data
- Click "Continue as [Name]" to complete social sign-in
- Or click "Cancel" to abort

#### Password Reset Testing
- Navigate to `/auth/forgot-password`
- Enter any email address
- See confirmation message (always succeeds in mock)
- Optional: Test reset flow at `/auth/reset-password?token=reset_123`

### 3. Authentication State

#### Persistence Testing
- Sign in with "Remember me" checked
- Refresh browser - should stay logged in
- Sign in without "Remember me"
- Refresh browser - should be logged out

#### Navigation Protection
- When logged out, try accessing `/` - should redirect to login
- When logged in, try accessing `/auth/login` - should redirect to dashboard
- Check user info in header (avatar, name, email)
- Use "Sign Out" from user menu

### 4. Mock API Behavior

#### Successful Logins
- `admin@example.com` / `password123`
- `user@example.com` / `password123`

#### Registration Conflicts
- Try registering with existing emails to see error handling

#### Network Simulation
- All requests have 700-1200ms delay to simulate real network
- Loading states are visible during requests

### 5. Component Integration

#### Auth Store Usage
```typescript
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

// Check authentication
console.log(authStore.isAuthenticated)
console.log(authStore.user)
console.log(authStore.token)

// Login
await authStore.login({ email, password, remember })

// Register  
await authStore.register({ name, email, password, confirmPassword, acceptTerms })

// Logout
authStore.logout()
```

## File Structure

```
src/
├── types/
│   └── auth.ts                 # TypeScript interfaces
├── data/
│   └── mockAuth.ts             # Mock API functions
├── stores/
│   └── auth.ts                 # Pinia auth store
├── components/auth/
│   ├── AuthCard.vue           # Reusable auth card wrapper
│   ├── PasswordField.vue      # Password input with show/hide
│   └── SocialLoginButton.vue  # Social provider button
├── views/auth/
│   ├── LoginView.vue          # Login page
│   ├── RegisterView.vue       # Registration page
│   ├── ForgotPasswordView.vue # Forgot password page
│   └── ResetPasswordView.vue  # Reset password page
└── router/
    └── index.ts               # Updated with auth routes & guards
```

## Customization

### Mock Data
Edit `src/data/mockAuth.ts` to:
- Change mock user credentials
- Modify social login providers
- Adjust network delay timing
- Customize validation rules

### Styling
Auth pages use the same theme system:
- Colors from `src/styles/variables.scss`
- Consistent with app-wide design
- Responsive breakpoints
- Dark/light mode support

### Real API Integration
Replace mock functions in `src/data/mockAuth.ts`:

```typescript
// Replace this:
export const mockLogin = async (credentials: LoginRequest): Promise<AuthResponse> => {
  // Mock implementation
}

// With this:
export const apiLogin = async (credentials: LoginRequest): Promise<AuthResponse> => {
  const response = await axios.post('/api/auth/login', credentials)
  return response.data
}
```

### OAuth Integration
Replace mock social login in components with real OAuth:

```typescript
// Replace mock modal with real OAuth redirect
const handleSocialLogin = (provider: 'google' | 'facebook') => {
  window.location.href = `${API_BASE}/auth/${provider}`
}
```

## Security Notes

- Tokens are stored in localStorage when "Remember me" is selected
- Session-only storage when "Remember me" is unchecked  
- Auth state is cleared on logout
- Navigation guards protect authenticated routes
- Form validation prevents common security issues

## Browser Support

- Chrome, Firefox, Safari, Edge (modern versions)
- Mobile responsive design
- Touch-friendly interactions
- Keyboard navigation support
- Screen reader compatible
