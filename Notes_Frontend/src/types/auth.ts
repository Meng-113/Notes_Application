export interface AuthPayload {
  Username: string
  Password: string
}

export interface AuthResponse {
  Username: string
  Token: string
  ExpiresAt: string
}
