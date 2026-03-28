using Note_Backend.Models.DTOs;
using Note_Backend.Models.Entity;

namespace Note_Backend.Repositories
{
    public interface INoteRepository
    {
        Task<List<NoteEntity>> GetAllAsync(int userId);
        Task<NoteEntity?> GetByIdAsync(int id, int userId);
        Task<NoteEntity> AddAsync(AddNoteDTO dto, int userId);
        Task<NoteEntity?> UpdateAsync(int id, UpdateNoteDTO dto, int userId);
        Task DeleteAsync(int id, int userId);
    }
}
