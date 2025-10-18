export interface User {
  id: string
  name: string
  email: string
  avatar?: string
}

export interface AuthToken {
  token: string
  expiresIn: number
}

export interface LoginRequest {
  email: string
  password: string
  remember?: boolean
}

export interface RegisterRequest {
  name: string
  email: string
  password: string
  confirmPassword: string
  acceptTerms: boolean
}

export interface ForgotPasswordRequest {
  email: string
}

export interface ResetPasswordRequest {
  token: string
  newPassword: string
  confirmPassword: string
}

export interface AuthResponse {
  ok: boolean
  user?: User
  token?: string
  error?: string
}

export interface SocialProvider {
  id: 'google' | 'facebook'
  name: string
  color: string
  icon: string
  mockUser: User
}

export type PasswordStrength = 'weak' | 'medium' | 'strong'

export interface PasswordValidation {
  strength: PasswordStrength
  score: number
  feedback: string[]
}
