<script setup lang="ts">
import { ChevronDown, Plus, Search, SlidersHorizontal } from 'lucide-vue-next'
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
  <aside class="flex h-full min-h-0 flex-col border-r border-neutral-900 bg-black">
    <div class="shrink-0 border-b border-neutral-900 bg-black px-4 pb-4 pt-6">
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

      <div class="space-y-2">
        <div class="flex items-center justify-between px-1"></div>

        <div class="relative">
          <select
            v-model="notesStore.filter"
            class="w-full appearance-none rounded-2xl border border-neutral-800 bg-neutral-950/90 py-3 pl-4 pr-12 text-sm font-medium text-white outline-none transition hover:border-neutral-700 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
          >
            <option value="all">All Notes</option>
            <option value="with-content">With Content</option>
            <option value="empty">Empty</option>
          </select>
          <ChevronDown
            class="pointer-events-none absolute right-4 top-1/2 h-4 w-4 -translate-y-1/2 text-neutral-500"
          />
        </div>
      </div>
    </div>

    <div
      class="flex-1 min-h-0 overflow-y-auto px-4 py-4 [scrollbar-width:none] [-ms-overflow-style:none] [&::-webkit-scrollbar]:hidden"
    >
      <div class="space-y-3">
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
    </div>
  </aside>
</template>
