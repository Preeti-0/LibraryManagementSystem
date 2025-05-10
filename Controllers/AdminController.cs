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
    }
}
