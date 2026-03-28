using System.Data.SqlClient;
using Dapper;
using Note_Backend.Models.Entity;

namespace Note_Backend.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IConfiguration _configuration;

        public UserAccountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<UserAccountEntity?> GetByUsernameAsync(string username)
        {
            using var connection = GetConnection();

            return await connection.QueryFirstOrDefaultAsync<UserAccountEntity>(
                "Select Top 1 * from UserAccounts where Username = @Username",
                new { Username = username });
        }

        public async Task<UserAccountEntity> CreateAsync(UserAccountEntity userAccount)
        {
            using var connection = GetConnection();

            var userId = await connection.QuerySingleAsync<int>(
                "Insert into UserAccounts (Username, PasswordHash) Output Inserted.Id values (@Username, @PasswordHash)",
                new
                {
                    userAccount.Username,
                    userAccount.PasswordHash
                });

            userAccount.Id = userId;
            return userAccount;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
