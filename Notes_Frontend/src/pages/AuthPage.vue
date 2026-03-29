<script setup lang="ts">
import { computed, ref } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const mode = ref<'login' | 'register'>('login')
const form = ref({
  Username: '',
  Password: '',
})

const buttonText = computed(() => {
  return mode.value === 'login' ? 'Login' : 'Create Account'
})

const helperText = computed(() => {
  return mode.value === 'login'
    ? 'Login with your username and password.'
    : 'Create a new username and password.'
})

function setMode(nextMode: 'login' | 'register') {
  mode.value = nextMode
  authStore.clearError()
}

async function submitForm() {
  const payload = {
    Username: form.value.Username.trim(),
    Password: form.value.Password,
  }

  if (mode.value === 'login') {
    await authStore.login(payload)
    return
  }

  await authStore.register(payload)
}
</script>

<template>
  <main class="flex min-h-screen items-center justify-center bg-black px-4 text-white">
    <section
      class="w-full max-w-md rounded-3xl border border-neutral-800 bg-neutral-950 p-8 shadow-2xl"
    >
      <p class="text-sm uppercase tracking-[0.3em] text-yellow-400">Notes App</p>
      <h1 class="mt-3 text-3xl font-bold">{{ buttonText }}</h1>
      <p class="mt-2 text-sm text-neutral-400">{{ helperText }}</p>

      <div class="mt-6 grid grid-cols-2 gap-2 rounded-2xl bg-black p-1">
        <button
          class="rounded-2xl px-4 py-3 text-sm font-medium transition"
          :class="mode === 'login' ? 'bg-yellow-400 text-black' : 'text-neutral-300'"
          @click="setMode('login')"
        >
          Login
        </button>
        <button
          class="rounded-2xl px-4 py-3 text-sm font-medium transition"
          :class="mode === 'register' ? 'bg-yellow-400 text-black' : 'text-neutral-300'"
          @click="setMode('register')"
        >
          Register
        </button>
      </div>

      <form class="mt-6 space-y-4" @submit.prevent="submitForm">
        <div>
          <label class="mb-2 block text-sm text-neutral-300">Username</label>
          <input
            v-model="form.Username"
            type="text"
            placeholder="Enter username"
            class="w-full rounded-2xl border border-neutral-800 bg-black px-4 py-3 text-white outline-none transition focus:border-yellow-400"
          />
        </div>

        <div>
          <label class="mb-2 block text-sm text-neutral-300">Password</label>
          <input
            v-model="form.Password"
            type="password"
            placeholder="Enter password"
            class="w-full rounded-2xl border border-neutral-800 bg-black px-4 py-3 text-white outline-none transition focus:border-yellow-400"
          />
        </div>

        <p v-if="authStore.errorMessage" class="text-sm text-red-400">
          {{ authStore.errorMessage }}
        </p>

        <button
          type="submit"
          :disabled="authStore.loading"
          class="w-full rounded-2xl bg-yellow-400 px-4 py-3 font-semibold text-black transition hover:brightness-95 disabled:opacity-60"
        >
          {{ authStore.loading ? 'Please wait...' : buttonText }}
        </button>
      </form>

      <p class="mt-5 text-xs text-neutral-500">
        If you do not have an account yet, click Register first.
      </p>
    </section>
  </main>
</template>
