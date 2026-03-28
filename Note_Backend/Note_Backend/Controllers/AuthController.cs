using Microsoft.AspNetCore.Mvc;
using Note_Backend.Models.Api;
using Note_Backend.Models.Entity;
using Note_Backend.Repositories;
using Note_Backend.Services;

namespace Note_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly PasswordService _passwordService;
        private readonly JwtService _jwtService;

        public AuthController(
            IUserAccountRepository userAccountRepository,
            PasswordService passwordService,
            JwtService jwtService)
        {
            _userAccountRepository = userAccountRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var username = request.Username.Trim();
            var password = request.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = await _userAccountRepository.GetByUsernameAsync(username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            var newUser = new UserAccountEntity
            {
                Username = username,
                PasswordHash = _passwordService.HashPassword(password)
            };

            var savedUser = await _userAccountRepository.CreateAsync(newUser);
            var response = _jwtService.CreateToken(savedUser);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var username = request.Username.Trim();
            var password = request.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _userAccountRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return Unauthorized("Username or password is wrong.");
            }

            var isPasswordCorrect = _passwordService.VerifyPassword(password, user.PasswordHash);
            if (!isPasswordCorrect)
            {
                return Unauthorized("Username or password is wrong.");
            }

            var response = _jwtService.CreateToken(user);
            return Ok(response);
        }
    }
}
