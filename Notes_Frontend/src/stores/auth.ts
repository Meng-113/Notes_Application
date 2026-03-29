import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import * as authApi from '@/api/auth'
import type { AuthPayload, AuthResponse } from '@/types/auth'

const TOKEN_KEY = 'notes_token'
const USERNAME_KEY = 'notes_username'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem(TOKEN_KEY) || '')
  const username = ref(localStorage.getItem(USERNAME_KEY) || '')
  const loading = ref(false)
  const errorMessage = ref('')

  const isLoggedIn = computed(() => token.value !== '')

  function clearError() {
    errorMessage.value = ''
  }

  function saveSession(response: AuthResponse) {
    token.value = response.Token
    username.value = response.Username

    localStorage.setItem(TOKEN_KEY, response.Token)
    localStorage.setItem(USERNAME_KEY, response.Username)
  }

  async function register(payload: AuthPayload) {
    loading.value = true
    clearError()

    try {
      const response = await authApi.register(payload)
      saveSession(response)
    } catch (error: unknown) {
      errorMessage.value = 'Could not create account.'
    } finally {
      loading.value = false
    }
  }

  async function login(payload: AuthPayload) {
    loading.value = true
    clearError()

    try {
      const response = await authApi.login(payload)
      saveSession(response)
    } catch (error: unknown) {
      errorMessage.value = 'Username or password is wrong.'
    } finally {
      loading.value = false
    }
  }

  function logout() {
    token.value = ''
    username.value = ''
    clearError()

    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USERNAME_KEY)
  }

  return {
    token,
    username,
    loading,
    errorMessage,
    isLoggedIn,
    clearError,
    register,
    login,
    logout,
  }
})
