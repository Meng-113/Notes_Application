namespace Note_Backend.Models.Entity
{
    public class UserAccountEntity
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
    }
}
