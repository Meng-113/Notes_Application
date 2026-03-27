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

        public async Task AddAsync(AddNoteDTO dto)
        {
            var connection = GetConnection();
            var Result = await connection.ExecuteAsync
                ("Insert into Notes (Title, Content) values(@Title, @Content)", dto);
        }

        public async Task DeleteAsync(int id)
        {
            var connection = GetConnection();
            var Result = await connection.ExecuteAsync
                ("Delete from Notes where Id = @Id", new { Id = id });
        }

        public async Task<List<NoteEntity>> GetAllAsync()
        {
            var connection = GetConnection();
            var Result = await connection.QueryAsync<NoteEntity>("Select * from Notes");
            return Result.ToList();
        }

        public async Task<NoteEntity> GetByIdAsync(int id)
        {
            var connection = GetConnection();
            var Result = await connection.QueryFirstOrDefaultAsync<NoteEntity>
                ("Select * from Notes where Id = @Id", new { Id = id });
            return Result;
        }

        public async Task UpdateAsync(int id, UpdateNoteDTO dto)
        {
            var connection = GetConnection();
            var result = await connection.ExecuteAsync("Update Notes set Title = @Title, Content = @Content where Id = @Id",
            new
            {
                Id = id,
                dto.Title,
                dto.Content
            }
            );
        }
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
