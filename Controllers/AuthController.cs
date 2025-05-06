using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Dto;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Registration failed: All fields are required.");
                return BadRequest(new { message = "All fields are required." });
            }

            if (_context.Users.Any(u => u.Email == dto.Email))
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", dto.Email);
                return BadRequest(new { message = "Email already exists." });
            }

            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists.", dto.Username);
                return BadRequest(new { message = "Username already exists." });
            }

            try
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                if (string.IsNullOrEmpty(passwordHash))
                {
                    _logger.LogError("BCrypt failed to generate a password hash for user {Username}.", dto.Username);
                    return StatusCode(500, new { message = "Failed to hash password." });
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = passwordHash
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Username} registered successfully.", dto.Username);
                return Ok(new { message = "Registration successful." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Username}.", dto.Username);
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Login failed: Email and password are required.");
                return BadRequest(new { message = "Email and password are required." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User with email {Email} not found.", dto.Email);
                return Unauthorized(new { message = "Invalid email or password." });
            }

            try
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Login failed: Invalid password for user {Email}.", dto.Email);
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("User {Email} logged in successfully.", dto.Email);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}.", dto.Email);
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}