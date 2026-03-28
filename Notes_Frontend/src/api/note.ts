import axios from 'axios'
import type { Note, NoteDraft } from '@/types/note'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

type NoteResponse = Partial<{
  id: number
  Id: number
  title: string
  Title: string
  content: string | null
  Content: string | null
  createdAt: string
  CreatedAt: string
  updatedAt: string
  UpdatedAt: string
}>

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null
}

function normalizeNote(note: NoteResponse): Note {
  return {
    Id: note.Id ?? note.id ?? 0,
    Title: note.Title ?? note.title ?? '',
    Content: note.Content ?? note.content ?? null,
    CreatedAt: note.CreatedAt ?? note.createdAt ?? '',
    UpdatedAt: note.UpdatedAt ?? note.updatedAt ?? '',
  }
}

export async function fetchNotes(): Promise<Note[]> {
  const response = await apiClient.get('/Notes')
  if (!Array.isArray(response.data)) return []
  return response.data.filter(isRecord).map((note) => normalizeNote(note))
}

export async function createNote(payload: NoteDraft): Promise<Note | null> {
  const response = await apiClient.post('/Notes', payload)
  return isRecord(response.data) ? normalizeNote(response.data) : null
}

export async function updateNote(id: number, payload: NoteDraft): Promise<Note | null> {
  const response = await apiClient.put(`/Notes/${id}`, payload)
  return isRecord(response.data) ? normalizeNote(response.data) : null
}

export async function deleteNote(id: number): Promise<void> {
  const response = await apiClient.delete(`/Notes/${id}`)
  return response.data
}
