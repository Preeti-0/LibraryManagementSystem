using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
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
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Review/add
        [HttpPost("add")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto dto)
        {
            var memberId = int.Parse(User.FindFirst("userId").Value);

            bool purchased = await _context.Orders
                .Include(o => o.OrderItems)
                .AnyAsync(o =>
                    o.MemberId == memberId &&
                    !o.IsCancelled &&
                    o.OrderItems.Any(i => i.BookId == dto.BookId));

            if (!purchased)
                return BadRequest("You can only review books you have purchased.");

            bool alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.MemberId == memberId && r.BookId == dto.BookId);

            if (alreadyReviewed)
                return BadRequest("You have already reviewed this book.");

            var review = new Review
            {
                MemberId = memberId,
                BookId = dto.BookId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok("Review submitted successfully.");
        }


        // GET: api/Review/book/2
        [AllowAnonymous]
        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetBookReviews(int bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.Member)
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    MemberName = r.Member.Name
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}
