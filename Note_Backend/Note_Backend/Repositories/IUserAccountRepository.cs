using Note_Backend.Models.Entity;

namespace Note_Backend.Repositories
{
    public interface IUserAccountRepository
    {
        Task<UserAccountEntity?> GetByUsernameAsync(string username);
        Task<UserAccountEntity> CreateAsync(UserAccountEntity userAccount);
    }
}
