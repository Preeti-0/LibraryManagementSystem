using LibraryManagementSystem.Configurations;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _jwtSettings = jwtSettings;
            _emailService = emailService;
            _configuration = configuration;
        }

        // ✅ Registration with email verification
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existing = await _context.Members.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing != null)
                return BadRequest("Email already exists.");

            var verificationToken = Guid.NewGuid().ToString();

            var member = new Member
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                MembershipId = Guid.NewGuid().ToString(),
                EmailVerificationToken = verificationToken,
                IsVerified = false,
                Role = "Member"
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            var baseUrl = _configuration["App:BaseUrl"];
            var link = $"{baseUrl}/api/Auth/verify-email?token={verificationToken}";
            var html = $"<h3>Welcome to the Library!</h3><p>Click <a href='{link}'>here</a> to verify your email.</p>";

            await _emailService.SendEmailAsync(member.Email, "Verify your Email", html);

            return Ok("Registration successful. Check your email to verify your account.");
        }

        // ✅ Login with verification check
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            if (!user.IsVerified)
                return Unauthorized("Please verify your email before logging in.");

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        // ✅ Verify email endpoint
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
            if (user == null)
                return BadRequest("Invalid or expired verification token.");

            user.IsVerified = true;
            user.EmailVerificationToken = null;
            await _context.SaveChangesAsync();

            return Content("<h2>Email verified successfully!</h2><p>You can now log in to your account.</p>", "text/html");
        }

        // ✅ Resend verification email
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromQuery] string email)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound("User not found.");
            if (user.IsVerified) return BadRequest("User already verified.");

            user.EmailVerificationToken = Guid.NewGuid().ToString();
            await _context.SaveChangesAsync();

            var baseUrl = _configuration["App:BaseUrl"];
            var link = $"{baseUrl}/api/Auth/verify-email?token={user.EmailVerificationToken}";
            var html = $"<p>Click <a href='{link}'>here</a> to verify your account.</p>";

            await _emailService.SendEmailAsync(user.Email, "Resend: Verify your Email", html);

            return Ok("Verification email resent. Please check your inbox.");
        }

        // ✅ JWT Token Generator
        private string GenerateToken(Member user)
        {
            var settings = _jwtSettings.Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("userId", user.Id.ToString()),
                new Claim("name", user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
