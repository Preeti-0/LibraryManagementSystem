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
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int memberId = int.Parse(User.FindFirst("userId").Value);

            var items = await _context.CartItems
                .Where(c => c.MemberId == memberId)
                .Include(c => c.Book)
                .Select(c => new
                {
                    c.Id,
                    c.BookId,
                    c.Quantity,
                    c.Book.BookName,
                    c.Book.ImageUrl,
                    c.Book.Price
                })
                .ToListAsync();

            return Ok(items);
        }

        // POST: api/Cart/add/2
        [HttpPost("add/{bookId}")]
        public async Task<IActionResult> AddToCart(int bookId)
        {
            int memberId = int.Parse(User.FindFirst("userId").Value);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.MemberId == memberId && c.BookId == bookId);

            if (item != null)
            {
                item.Quantity += 1;
            }
            else
            {
                item = new CartItem
                {
                    MemberId = memberId,
                    BookId = bookId,
                    Quantity = 1
                };
                _context.CartItems.Add(item);
            }

            await _context.SaveChangesAsync();
            return Ok("Book added to cart.");
        }

        // DELETE: api/Cart/remove/2
        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> RemoveFromCart(int bookId)
        {
            int memberId = int.Parse(User.FindFirst("userId").Value);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.MemberId == memberId && c.BookId == bookId);

            if (item == null) return NotFound("Item not found in cart.");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Book removed from cart.");
        }
    }
}
