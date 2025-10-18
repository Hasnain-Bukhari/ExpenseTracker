import type {
  User,
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  SocialProvider,
  PasswordValidation,
  PasswordStrength
} from '@/types/auth'

// Mock delay to simulate network latency
const mockDelay = () => new Promise(resolve => 
  setTimeout(resolve, Math.random() * 500 + 700) // 700-1200ms
)

// Mock users database
const mockUsers: User[] = [
  {
    id: '1',
    name: 'John Doe',
    email: 'john@example.com',
    avatar: 'https://i.pravatar.cc/150?img=1'
  },
  {
    id: '2',
    name: 'Jane Smith',
    email: 'jane@example.com',
    avatar: 'https://i.pravatar.cc/150?img=2'
  }
]

// Social providers configuration
export const socialProviders: SocialProvider[] = [
  {
    id: 'google',
    name: 'Google',
    color: '#4285F4',
    icon: 'mdi-google',
    mockUser: {
      id: 'google_123',
      name: 'John Doe',
      email: 'john.doe@gmail.com',
      avatar: 'https://i.pravatar.cc/150?img=3'
    }
  },
  {
    id: 'facebook',
    name: 'Facebook',
    color: '#1877F2',
    icon: 'mdi-facebook',
    mockUser: {
      id: 'facebook_456',
      name: 'Jane Smith',
      email: 'jane.smith@facebook.com',
      avatar: 'https://i.pravatar.cc/150?img=4'
    }
  }
]

// Mock login API
export const mockLogin = async (credentials: LoginRequest): Promise<AuthResponse> => {
  await mockDelay()
  
  // TODO: Replace with real API call
  // const response = await axios.post('/api/auth/login', credentials)
  
  // Mock validation
  if (credentials.email === 'admin@example.com' && credentials.password === 'password123') {
    return {
      ok: true,
      user: {
        id: '1',
        name: 'Hasnain Bukhari',
        email: credentials.email,
        avatar: 'https://i.pravatar.cc/150?img=1'
      },
      token: 'mock.jwt.token.' + Date.now()
    }
  }
  
  if (credentials.email === 'user@example.com' && credentials.password === 'password123') {
    return {
      ok: true,
      user: mockUsers[0],
      token: 'mock.jwt.token.' + Date.now()
    }
  }
  
  return {
    ok: false,
    error: 'Invalid email or password'
  }
}

// Mock register API
export const mockRegister = async (data: RegisterRequest): Promise<AuthResponse> => {
  await mockDelay()
  
  // TODO: Replace with real API call
  // const response = await axios.post('/api/auth/register', data)
  
  // Check if email already exists
  if (mockUsers.some(user => user.email === data.email) || data.email === 'admin@example.com') {
    return {
      ok: false,
      error: 'An account with this email already exists'
    }
  }
  
  // Mock successful registration
  const newUser: User = {
    id: 'user_' + Date.now(),
    name: data.name,
    email: data.email,
    avatar: `https://i.pravatar.cc/150?img=${Math.floor(Math.random() * 70) + 1}`
  }
  
  return {
    ok: true,
    user: newUser,
    token: 'mock.jwt.token.' + Date.now()
  }
}

// Mock forgot password API
export const mockForgotPassword = async (_data: ForgotPasswordRequest): Promise<AuthResponse> => {
  await mockDelay()
  
  // TODO: Replace with real API call
  // const response = await axios.post('/api/auth/forgot-password', _data)
  
  // Always return success for demo purposes
  return {
    ok: true
  }
}

// Mock reset password API
export const mockResetPassword = async (data: ResetPasswordRequest): Promise<AuthResponse> => {
  await mockDelay()
  
  // TODO: Replace with real API call
  // const response = await axios.post('/api/auth/reset-password', data)
  
  // Mock token validation - accept any token starting with 'reset_'
  if (!data.token.startsWith('reset_')) {
    return {
      ok: false,
      error: 'Invalid or expired reset token'
    }
  }
  
  return {
    ok: true
  }
}

// Mock social login
export const mockSocialLogin = async (provider: 'google' | 'facebook'): Promise<AuthResponse> => {
  await mockDelay()
  
  // TODO: Replace with real OAuth flow
  // window.location.href = `https://oauth.${provider}.com/authorize?...`
  
  const socialProvider = socialProviders.find(p => p.id === provider)
  if (!socialProvider) {
    return {
      ok: false,
      error: 'Unknown provider'
    }
  }
  
  return {
    ok: true,
    user: socialProvider.mockUser,
    token: `mock.${provider}.token.` + Date.now()
  }
}

// Password strength validator
export const validatePasswordStrength = (password: string): PasswordValidation => {
  let score = 0
  const feedback: string[] = []
  
  if (password.length >= 8) score += 1
  else feedback.push('At least 8 characters')
  
  if (/[A-Z]/.test(password)) score += 1
  else feedback.push('At least one uppercase letter')
  
  if (/[a-z]/.test(password)) score += 1
  else feedback.push('At least one lowercase letter')
  
  if (/\d/.test(password)) score += 1
  else feedback.push('At least one number')
  
  if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\?]/.test(password)) score += 1
  else feedback.push('At least one special character')
  
  let strength: PasswordStrength
  if (score <= 2) strength = 'weak'
  else if (score <= 3) strength = 'medium'
  else strength = 'strong'
  
  return { strength, score, feedback }
}
