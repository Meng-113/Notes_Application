<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useNotesStore } from '@/stores/notes'
import NotesSidebar from '@/components/notes/NotesSidebar.vue'
import NoteEditor from '@/components/notes/NoteEditor.vue'
import type { Note } from '@/types/note'

const notesStore = useNotesStore()
const mobileView = ref<'list' | 'editor'>('list')

onMounted(() => {
  notesStore.loadNotes()
})

function handleSelectNote(note: Note) {
  notesStore.selectNote(note)
  mobileView.value = 'editor'
}

function handleCreateNote() {
  notesStore.startCreating()
  mobileView.value = 'editor'
}

function handleBackToList() {
  mobileView.value = 'list'
}

async function handleDeleteNote() {
  await notesStore.deleteNote()
  mobileView.value = 'list'
}

async function handleSaveNote() {
  await notesStore.saveNote()
  if (!notesStore.errorMessage) {
    mobileView.value = 'list'
  }
}
</script>

<template>
  <main class="h-screen overflow-hidden bg-black text-white">
    <div class="hidden h-full min-h-0 md:grid md:grid-cols-[360px_1fr]">
      <NotesSidebar @select-note="handleSelectNote" @create-note="handleCreateNote" />
      <NoteEditor @save="handleSaveNote" @delete="handleDeleteNote" />
    </div>

    <div class="h-full min-h-0 md:hidden">
      <NotesSidebar
        v-if="mobileView === 'list'"
        @select-note="handleSelectNote"
        @create-note="handleCreateNote"
      />
      <NoteEditor
        v-else
        :show-back-button="true"
        @back="handleBackToList"
        @save="handleSaveNote"
        @delete="handleDeleteNote"
      />
    </div>
  </main>
</template>
