using System.Security.Cryptography;
using System.Text;

namespace Note_Backend.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string savedPasswordHash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == savedPasswordHash;
        }
    }
}
