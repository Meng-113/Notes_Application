using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Note_Backend.Models.Api;
using Note_Backend.Models.Entity;

namespace Note_Backend.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LoginResponse CreateToken(UserAccountEntity userAccount)
        {
            var issuer = _configuration["JwtConfig:Issuer"] ?? string.Empty;
            var audience = _configuration["JwtConfig:Audience"] ?? string.Empty;
            var key = _configuration["JwtConfig:Key"] ?? string.Empty;
            var tokenValidityMins = int.Parse(_configuration["JwtConfig:TokenValidityMins"] ?? "30");
            var expiresAt = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
                new(ClaimTypes.Name, userAccount.Username)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new LoginResponse
            {
                Username = userAccount.Username,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt.ToString("O")
            };
        }
    }
}
