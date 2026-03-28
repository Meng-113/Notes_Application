import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import * as noteApi from '@/api/note'
import type { Note, NoteDraft, FilterType } from '@/types/note'

export const useNotesStore = defineStore('notes', () => {
  const notes = ref<Note[]>([])
  const selectedId = ref<number | null>(null)

  const loading = ref(false)
  const saving = ref(false)
  const errorMessage = ref('')

  const filter = ref<FilterType>('all')
  const search = ref('')

  const draft = ref<NoteDraft>({
    Title: '',
    Content: '',
  })

  const selectedNote = computed(() => {
    return notes.value.find((note) => note.Id == selectedId.value) || null
  })

  const visibleNotes = computed(() => {
    const query = search.value.trim().toLowerCase()
    return notes.value.filter((note) => {
      const Content = note.Content || ''

      const matchesSearch =
        query === '' ||
        note.Title.toLowerCase().includes(query) ||
        Content.toLowerCase().includes(query)

      if (!matchesSearch) return false

      if (filter.value === 'with-content') return Content.trim().length > 0
      if (filter.value === 'empty') return Content.trim().length === 0
      return true
    })
  })

  const editNote = computed({
    get: () => {
      const title = draft.value.Title || ''
      const content = draft.value.Content || ''
      return content ? `${title}\n${content}` : title
    },
    set: (value: string) => {
      const normalized = value.replace(/\r\n/g, '\n')
      const lines = normalized.split('\n')

      draft.value.Title = lines[0] || ''
      draft.value.Content = lines.slice(1).join('\n') || ''
    },
  })

  function clearError() {
    errorMessage.value = ''
  }

  function startCreating() {
    selectedId.value = null
    draft.value = {
      Title: '',
      Content: '',
    }
    clearError()
  }

  function selectNote(note: Note) {
    selectedId.value = note.Id
    draft.value = {
      Title: note.Title,
      Content: note.Content || '',
    }
    clearError()
  }

  async function loadNotes() {
    loading.value = true
    clearError()
    try {
      notes.value = await noteApi.fetchNotes()
    } catch (error) {
      errorMessage.value = 'Failed to load notes.'
    } finally {
      loading.value = false
    }
  }

  async function saveNote() {
    const title = draft.value.Title.trim()
    if (!title) {
      errorMessage.value = 'Title cannot be empty.'
      return
    }

    saving.value = true
    clearError()

    try {
      const payload: NoteDraft = {
        Title: title,
        Content: draft.value.Content,
      }
      if (selectedId.value !== null) {
        await noteApi.updateNote(selectedId.value, payload)
      } else {
        const created = await noteApi.createNote(payload)
        selectedId.value = created?.Id ?? null
      }

      await loadNotes()

      if (selectedId.value !== null) {
        const lated = notes.value.find((note) => note.Id === selectedId.value)
        if (lated) {
          draft.value = {
            Title: lated.Title,
            Content: lated.Content || '',
          }
        }
      }
    } catch (error) {
      errorMessage.value = 'Failed to save note.'
    } finally {
      saving.value = false
    }
  }

  async function deleteNote() {
    if (selectedId.value === null) return
    saving.value = true
    clearError()

    try {
      await noteApi.deleteNote(selectedId.value)
      await loadNotes()
      startCreating()
    } catch (error) {
      errorMessage.value = 'Failed to delete note.'
    } finally {
      saving.value = false
    }
  }
  return {
    notes,
    selectedId,
    selectedNote,
    loading,
    saving,
    errorMessage,
    filter,
    search,
    draft,
    visibleNotes,
    editNote,
    startCreating,
    selectNote,
    loadNotes,
    saveNote,
    deleteNote,
  }
})
