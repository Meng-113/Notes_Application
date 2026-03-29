import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import * as noteApi from '@/api/note'
import type { FilterType, Note, NoteDraft, NoteSection } from '@/types/note'

const DAY_IN_MS = 24 * 60 * 60 * 1000

function getValidDate(value: string) {
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

function getNoteDate(note: Note) {
  return getValidDate(note.UpdatedAt) ?? getValidDate(note.CreatedAt) ?? new Date(0)
}

function getStartOfDay(date: Date) {
  const normalized = new Date(date)
  normalized.setHours(0, 0, 0, 0)
  return normalized
}

function getSectionMeta(date: Date, now: Date) {
  const diffInDays = Math.floor(
    (getStartOfDay(now).getTime() - getStartOfDay(date).getTime()) / DAY_IN_MS,
  )

  if (diffInDays <= 0) {
    return {
      id: 'today',
      label: 'Today',
    }
  }

  if (diffInDays <= 7) {
    return {
      id: 'previous-7-days',
      label: 'Previous 7 Days',
    }
  }

  if (diffInDays <= 30) {
    return {
      id: 'previous-30-days',
      label: 'Previous 30 Days',
    }
  }

  return {
    id: `year-${date.getFullYear()}`,
    label: String(date.getFullYear()),
  }
}

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
    return [...notes.value]
      .filter((note) => {
        const content = note.Content || ''

        const matchesSearch =
          query === '' ||
          note.Title.toLowerCase().includes(query) ||
          content.toLowerCase().includes(query)

        if (!matchesSearch) return false

        if (filter.value === 'with-content') return content.trim().length > 0
        if (filter.value === 'empty') return content.trim().length === 0
        return true
      })
      .sort((left, right) => {
        const timeDifference = getNoteDate(right).getTime() - getNoteDate(left).getTime()

        if (timeDifference !== 0) {
          return timeDifference
        }

        return right.Id - left.Id
      })
  })

  const groupedNotes = computed<NoteSection[]>(() => {
    const sections = new Map<string, NoteSection>()
    const result: NoteSection[] = []
    const now = new Date()

    for (const note of visibleNotes.value) {
      const section = getSectionMeta(getNoteDate(note), now)

      if (!sections.has(section.id)) {
        const nextSection: NoteSection = {
          id: section.id,
          label: section.label,
          notes: [],
        }

        sections.set(section.id, nextSection)
        result.push(nextSection)
      }

      sections.get(section.id)?.notes.push(note)
    }

    return result
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

  function resetState() {
    notes.value = []
    selectedId.value = null
    loading.value = false
    saving.value = false
    errorMessage.value = ''
    filter.value = 'all'
    search.value = ''
    draft.value = {
      Title: '',
      Content: '',
    }
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
        const latest = notes.value.find((note) => note.Id === selectedId.value)
        if (latest) {
          draft.value = {
            Title: latest.Title,
            Content: latest.Content || '',
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
    groupedNotes,
    editNote,
    resetState,
    startCreating,
    selectNote,
    loadNotes,
    saveNote,
    deleteNote,
  }
})
