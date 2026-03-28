using Note_Backend.Models.DTOs;
using Note_Backend.Models.Entity;

namespace Note_Backend.Repositories
{
    public interface INoteRepository
    {
        Task<List<NoteEntity>> GetAllAsync();
        Task<NoteEntity?> GetByIdAsync(int id);
        Task<NoteEntity> AddAsync(AddNoteDTO dto);
        Task<NoteEntity?> UpdateAsync(int id, UpdateNoteDTO dto);
        Task DeleteAsync(int id);
    }
}
