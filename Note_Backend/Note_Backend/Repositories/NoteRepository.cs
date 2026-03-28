using System.Data.SqlClient;
using Dapper;
using Note_Backend.Models.DTOs;
using Note_Backend.Models.Entity;

namespace Note_Backend.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly IConfiguration _configuration;

        public NoteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<NoteEntity> AddAsync(AddNoteDTO dto, int userId)
        {
            using var connection = GetConnection();
            var noteId = await connection.QuerySingleAsync<int>(
                "Insert into Notes (UserId, Title, Content) Output Inserted.Id values (@UserId, @Title, @Content)",
                new
                {
                    UserId = userId,
                    dto.Title,
                    dto.Content
                });

            var note = await GetByIdAsync(noteId, userId);
            return note!;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(
                "Delete from Notes where Id = @Id and UserId = @UserId",
                new
                {
                    Id = id,
                    UserId = userId
                });
        }

        public async Task<List<NoteEntity>> GetAllAsync(int userId)
        {
            using var connection = GetConnection();
            var result = await connection.QueryAsync<NoteEntity>(
                "Select * from Notes where UserId = @UserId order by UpdatedAt desc, Id desc",
                new { UserId = userId });

            return result.ToList();
        }

        public async Task<NoteEntity?> GetByIdAsync(int id, int userId)
        {
            using var connection = GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<NoteEntity>(
                "Select * from Notes where Id = @Id and UserId = @UserId",
                new
                {
                    Id = id,
                    UserId = userId
                });

            return result;
        }

        public async Task<NoteEntity?> UpdateAsync(int id, UpdateNoteDTO dto, int userId)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(
                "Update Notes set Title = @Title, Content = @Content, UpdatedAt = GETDATE() where Id = @Id and UserId = @UserId",
                new
                {
                    Id = id,
                    UserId = userId,
                    dto.Title,
                    dto.Content
                });

            return await GetByIdAsync(id, userId);
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
