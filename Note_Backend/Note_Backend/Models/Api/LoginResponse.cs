namespace Note_Backend.Models.Api
{
    public class LoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string ExpiresAt { get; set; } = string.Empty;
    }
}
