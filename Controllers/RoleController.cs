using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateUserRole(RoleUpdateDto dto)
        {
            var user = await _context.Members.FindAsync(dto.MemberId);
            if (user == null) return NotFound("User not found.");

            user.Role = dto.Role;
            await _context.SaveChangesAsync();

            return Ok($"Role updated to {dto.Role}");
        }
    }
}
