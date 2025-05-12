using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BannerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin can create a banner
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBanner([FromBody] BannerDto dto)
        {
            var banner = new Banner
            {
                Message = dto.Message,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return Ok("Banner created successfully.");
        }


        // Anyone can view active banners
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBanners()
        {
            var now = DateTime.UtcNow;

            var banners = await _context.Banners
                .Where(b => b.IsActive && b.StartDate <= now && b.EndDate >= now)
                .ToListAsync();

            return Ok(banners);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBanner(int id, [FromBody] BannerDto dto)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
                return NotFound("Banner not found.");

            banner.Message = dto.Message;
            banner.StartDate = dto.StartDate;
            banner.EndDate = dto.EndDate;
            banner.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return Ok("Banner updated successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
                return NotFound("Banner not found.");

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return Ok("Banner deleted successfully.");
        }

    }
}
