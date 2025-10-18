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

// API-facing DTOs (integration prompt)
export interface RegisterRequestDto { name: string; email: string; phone?: string | null; password?: string | null; acceptTerms: boolean }
export interface LoginRequestDto { email: string; password: string; remember: boolean }
export interface SocialLoginRequestDto { provider: string; token: string; remember: boolean }
export interface RefreshRequestDto { refreshToken: string }
export interface ForgotPasswordRequestDto { email: string }
export interface ResetPasswordRequestDto { token: string; newPassword: string }

export interface AuthUserDto { id: string; name: string; email: string }
export interface AuthApiResponse { ok: boolean; user?: AuthUserDto; accessToken?: string; refreshToken?: string; expiresIn?: number }
