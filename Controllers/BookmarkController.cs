using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public class BookmarkController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookmarkController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Bookmark
        [HttpGet]
        public async Task<IActionResult> GetBookmarks()
        {
            var memberId = int.Parse(User.FindFirst("userId").Value);

            var bookmarks = await _context.Bookmarks
                .Where(b => b.MemberId == memberId)
                .Include(b => b.Book)
                .Select(b => new
                {
                    b.Id,
                    b.BookId,
                    b.Book.BookName,
                    b.Book.ImageUrl,
                    b.Book.Price,
                    b.Book.Writer,
                    b.Book.Genre,
                    b.Book.ReleaseDate
                })
                .ToListAsync();

            return Ok(bookmarks);
        }

        // POST: api/Bookmark/add/5
        [HttpPost("add/{bookId}")]
        public async Task<IActionResult> AddToBookmark(int bookId)
        {
            var memberId = int.Parse(User.FindFirst("userId").Value);

            bool exists = await _context.Bookmarks
                .AnyAsync(b => b.BookId == bookId && b.MemberId == memberId);
            if (exists) return BadRequest("Book already bookmarked.");

            var bookmark = new Bookmark
            {
                BookId = bookId,
                MemberId = memberId
            };

            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            return Ok("Book bookmarked successfully.");
        }

        // DELETE: api/Bookmark/remove/5
        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> RemoveBookmark(int bookId)
        {
            var memberId = int.Parse(User.FindFirst("userId").Value);

            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.MemberId == memberId);

            if (bookmark == null) return NotFound("Bookmark not found.");

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            return Ok("Bookmark removed.");
        }
    }
}
