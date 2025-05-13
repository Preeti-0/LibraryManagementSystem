using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // ✅ Only Admins can use this controller
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] RoleUpdateDto dto)
        {
            var user = await _context.Members.FindAsync(dto.MemberId);
            if (user == null) return NotFound("User not found.");

            user.Role = dto.Role;
            await _context.SaveChangesAsync();

            return Ok($"User {user.Name}'s role updated to {user.Role}");
        }

        // ✅ New: Get list of users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Members
                .Select(u => new
                {
                    u.Id,              //  Correct field
                    u.Name,
                    u.Email,
                    u.Role,
                    u.IsVerified       //  Correct verification field
                })
                .ToListAsync();

            return Ok(users);
        }


        // ✅ New: Delete a user by id
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Members.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            _context.Members.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }

    }
}
