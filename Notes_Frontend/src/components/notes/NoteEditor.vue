<script setup lang="ts">
import { ArrowLeft, Save, Trash2 } from 'lucide-vue-next'
import { computed } from 'vue'
import { useNotesStore } from '@/stores/notes'

withDefaults(
  defineProps<{
    showBackButton?: boolean
  }>(),
  {
    showBackButton: false,
  },
)

const emit = defineEmits<{
  (e: 'back'): void
  (e: 'save'): void
  (e: 'delete'): void
}>()

const notesStore = useNotesStore()

const isEditing = computed(() => notesStore.selectedId !== null)

function formatDateTime(value: string) {
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? 'Unknown date' : date.toLocaleString()
}
</script>

<template>
  <section class="flex min-h-screen flex-col bg-black">
    <div class="flex items-center justify-between border-b border-neutral-900 px-4 py-3">
      <button
        v-if="showBackButton"
        @click="emit('back')"
        class="rounded-full p-2 text-neutral-300 transition hover:bg-neutral-900"
        aria-label="Back"
      >
        <ArrowLeft class="h-5 w-5" />
      </button>
      <div v-else class="h-9 w-9"></div>

      <div class="flex items-center gap-2">
        <button
          v-if="isEditing"
          @click="emit('delete')"
          class="rounded-full p-2 text-red-400 transition hover:bg-neutral-900"
          aria-label="Delete note"
        >
          <Trash2 class="h-5 w-5" />
        </button>

        <button
          @click="emit('save')"
          :disabled="notesStore.saving"
          class="rounded-full p-2 text-yellow-400 transition hover:bg-neutral-900 disabled:opacity-50"
          aria-label="Save note"
        >
          <Save class="h-5 w-5" />
        </button>
      </div>
    </div>

    <textarea
      v-model="notesStore.editNote"
      placeholder=""
      class="flex-1 resize-none bg-black px-5 py-5 text-lg leading-8 text-white outline-none placeholder:text-neutral-600"
    />

    <div class="border-t border-neutral-900 px-5 py-4 text-sm text-neutral-500">
      <template v-if="notesStore.selectedNote">
        <p>Created: {{ formatDateTime(notesStore.selectedNote.CreatedAt) }}</p>
        <p class="mt-1">Updated: {{ formatDateTime(notesStore.selectedNote.UpdatedAt) }}</p>
      </template>

      <p v-if="notesStore.errorMessage" class="mt-3 text-red-400">
        {{ notesStore.errorMessage }}
      </p>
    </div>
  </section>
</template>
