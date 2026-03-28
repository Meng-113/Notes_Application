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
</script>

<template>
  <section class="flex h-full min-h-0 flex-col bg-black">
    <div class="shrink-0 flex items-center justify-between border-b border-neutral-900 px-4 py-3">
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
      class="flex-1 min-h-0 overflow-y-auto resize-none bg-black px-5 py-5 text-lg leading-8 text-white outline-none placeholder:text-neutral-600"
    />
  </section>
</template>
