<script setup lang="ts">
import { Plus, Search } from 'lucide-vue-next'
import { useNotesStore } from '@/stores/notes'
import type { Note } from '@/types/note'

const emit = defineEmits<{
  (e: 'select-note', note: Note): void
  (e: 'create-note'): void
}>()

const notesStore = useNotesStore()

function formatDate(value: string) {
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? 'Unknown date' : date.toLocaleDateString()
}

function getPreview(note: Note) {
  return (note.Content || '').trim().slice(0, 100)
}
</script>

<template>
  <aside class="flex flex-col min-h-screen border-r border-neutral-900 bg-black">
    <div
      class="sticky top-0 z-0 border-b border-neutral-900 bg-black/95 px-4 pb-4 pt-6 backdrop-blur"
    >
      <div class="mb-4 flex items-center justify-between">
        <h1 class="text-3xl font-bold tracking-tight">My Notes</h1>
        <button
          class="rounded-full p-2 text-yellow-400 transition hover:bg-neutral-900"
          aria-label="Create note"
          @click="emit('create-note')"
        >
          <Plus class="w-6 h-6" />
        </button>
      </div>
      <div class="relative mb-3">
        <Search
          class="pointer-events-none absolute left-3 top-1/2 w-4 h-4 -translate-y-1/2 text-neutral-500"
        />
        <input
          type="text"
          v-model="notesStore.search"
          placeholder="Search"
          class="w-full rounded-2xl bg-neutral-900 py-3 pl-10 pr-4 text-white outline-none placeholder:text-neutral-500"
        />
      </div>

      <select
        v-model="notesStore.filter"
        class="w-full rounded-2xl bg-neutral-900 px-4 py-3 text-sm text-white outline-none"
      >
        <option value="all">All Notes</option>
        <option value="with-content">With Content</option>
        <option value="empty">Empty</option>
      </select>
    </div>

    <div class="flex-1 space-y-3 px-4 py-4">
      <p v-if="notesStore.loading" class="text-sm text-neutral-400">Loading notes...</p>
      <p v-else-if="notesStore.visibleNotes.length === 0" class="text-sm text-neutral-500">
        No notes found.
      </p>

      <button
        v-for="note in notesStore.visibleNotes"
        :key="note.Id"
        @click="emit('select-note', note)"
        class="w-full rounded-2xl border border-transparent bg-neutral-900 p-4 text-left transition hover:bg-neutral-800"
        :class="{
          'ring-1 ring-yellow-400': notesStore.selectedId === note.Id,
        }"
      >
        <p class="truncate text-base font-semibold text-white">
          {{ note.Title }}
        </p>

        <p v-if="getPreview(note)" class="mt-1 line-clamp-2 text-sm text-neutral-400">
          {{ getPreview(note) }}
        </p>

        <p class="mt-3 text-xs text-neutral-500">
          {{ formatDate(note.CreatedAt) }}
        </p>
      </button>
    </div>
  </aside>
</template>
