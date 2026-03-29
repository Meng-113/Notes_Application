<script setup lang="ts">
import AuthPage from '@/pages/AuthPage.vue'
import NotesPage from '@/pages/NotesPage.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
</script>

<template>
  <AuthPage v-if="!authStore.isLoggedIn" />
  <NotesPage v-else />
</template>
