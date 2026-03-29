export interface Note {
  Id: number
  Title: string
  Content: string | null
  CreatedAt: string
  UpdatedAt: string
}

export interface NoteDraft {
  Title: string
  Content: string | null
}

export type FilterType = 'all' | 'with-content' | 'empty'

export interface NoteSection {
  id: string
  label: string
  notes: Note[]
}
