import axios from 'axios'
import type { AuthPayload, AuthResponse } from '@/types/auth'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

export async function register(payload: AuthPayload): Promise<AuthResponse> {
  const response = await apiClient.post('/Auth/register', payload)
  return response.data as AuthResponse
}

export async function login(payload: AuthPayload): Promise<AuthResponse> {
  const response = await apiClient.post('/Auth/login', payload)
  return response.data as AuthResponse
}
