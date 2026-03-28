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

        public async Task<NoteEntity> AddAsync(AddNoteDTO dto)
        {
            using var connection = GetConnection();
            var noteId = await connection.QuerySingleAsync<int>(
                """
                Insert into Notes (Title, Content)
                Output Inserted.Id
                values(@Title, @Content)
                """,
                dto);

            var note = await GetByIdAsync(noteId);
            return note!;
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync("Delete from Notes where Id = @Id", new { Id = id });
        }

        public async Task<List<NoteEntity>> GetAllAsync()
        {
            using var connection = GetConnection();
            var result = await connection.QueryAsync<NoteEntity>("Select * from Notes");
            return result.ToList();
        }

        public async Task<NoteEntity?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<NoteEntity>(
                "Select * from Notes where Id = @Id",
                new { Id = id });
            return result;
        }

        public async Task<NoteEntity?> UpdateAsync(int id, UpdateNoteDTO dto)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(
                "Update Notes set Title = @Title, Content = @Content where Id = @Id",
                new
                {
                    Id = id,
                    dto.Title,
                    dto.Content
                });

            return await GetByIdAsync(id);
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
