using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Dto;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/book
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .OrderBy(b => b.Id);

            var totalCount = await query.CountAsync();

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    BookName = b.BookName,
                    Writer = b.Writer,
                    Genre = b.Genre,
                    ReleaseDate = b.ReleaseDate,
                    CreatedAt = b.CreatedAt,
                    ISBN = b.ISBN,
                    EditionType = b.EditionType,
                    PhotoPath = b.ImageUrl,
                    Price = b.Price,
                    Language = b.Language,
                    Format = b.Format,
                    PublisherName = b.Publisher.Name,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
                    IsOnSale = b.IsOnSale,
                    DiscountPercentage = b.DiscountPercentage,
                    DiscountStartDate = b.DiscountStartDate,
                    DiscountEndDate = b.DiscountEndDate,
                    Stock = b.Stock,
                    IsAwardWinner = b.IsAwardWinner,
                    IsAvailableInLibrary = b.IsAvailableInLibrary,
                    SalesCount = b.SalesCount
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Books = books
            });
        }


        // GET: api/book/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return book;
        }

        // POST: api/book
        // POST: api/book
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromForm] BookCreateDto dto)
        {
            string imageUrl = "";
            if (dto.BookImage != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.BookImage.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.BookImage.CopyToAsync(stream);
                }

                imageUrl = "/images/" + fileName;
            }

            var book = new Book
            {
                BookName = dto.BookName,
                Writer = dto.Writer,
                Genre = dto.Genre,
                ReleaseDate = DateTime.SpecifyKind(dto.ReleaseDate, DateTimeKind.Utc),
                ImageUrl = imageUrl,
                Price = dto.Price,
                Language = dto.Language,
                Format = dto.Format,
                PublisherId = dto.PublisherId,
                Stock = dto.Stock,
                Description = dto.Description,
                IsOnSale = false,

                // ✅ NEW FIELDS
                ISBN = dto.ISBN,
                EditionType = dto.EditionType,
                IsAvailableInLibrary = dto.IsAvailableInLibrary,
                CreatedAt = DateTime.UtcNow
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }


        // PUT: api/book/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched book ID.");

            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound("Book not found.");

            if (dto.BookImage != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.BookImage.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.BookImage.CopyToAsync(stream);
                }

                book.ImageUrl = "/images/" + fileName;
            }

            book.BookName = dto.BookName;
            book.Writer = dto.Writer;
            book.Genre = dto.Genre;
            book.ReleaseDate = DateTime.SpecifyKind(dto.ReleaseDate, DateTimeKind.Utc);
            book.Price = dto.Price;
            book.Language = dto.Language;
            book.Format = dto.Format;
            book.PublisherId = dto.PublisherId;
            book.Stock = dto.Stock;
            book.Description = dto.Description;
            book.IsOnSale = dto.IsOnSale;
            book.DiscountPercentage = dto.DiscountPercentage;
            book.DiscountStartDate = dto.DiscountStartDate;
            book.DiscountEndDate = dto.DiscountEndDate;
            book.IsAvailableInLibrary = dto.IsAvailableInLibrary;
            await _context.SaveChangesAsync();

            return Ok(book);
        }

        // DELETE: api/book/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-discount/{id}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] DiscountDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound("Book not found.");

            book.IsOnSale = dto.IsOnSale;
            book.DiscountPercentage = dto.DiscountPercentage;
            book.DiscountStartDate = dto.DiscountStartDate;
            book.DiscountEndDate = dto.DiscountEndDate;

            await _context.SaveChangesAsync();

            return Ok("Discount updated successfully.");
        }


        [AllowAnonymous]
        [HttpGet("new-releases")]
        public async Task<IActionResult> GetNewReleases()
        {
            var cutoff = DateTime.UtcNow.AddMonths(-3);
            var books = await _context.Books
                .Where(b => b.ReleaseDate >= cutoff)
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .ToListAsync();

            return Ok(books);
        }

        [AllowAnonymous]
        [HttpGet("new-arrivals")]
        public async Task<IActionResult> GetNewArrivals()
        {
            var cutoff = DateTime.UtcNow.AddMonths(-1);
            var books = await _context.Books
                .Where(b => b.CreatedAt >= cutoff)
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .ToListAsync();

            return Ok(books);
        }

        [AllowAnonymous]
        [HttpGet("bestsellers")]
        public async Task<IActionResult> GetBestsellers()
        {
            var books = await _context.Books
                .OrderByDescending(b => b.SalesCount)
                .Take(20)
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .ToListAsync();

            return Ok(books);
        }

        [AllowAnonymous]
        [HttpGet("deals")]
        public async Task<IActionResult> GetDeals()
        {
            var today = DateTime.UtcNow;
            var books = await _context.Books
                .Where(b => b.IsOnSale && b.DiscountStartDate <= today && b.DiscountEndDate >= today)
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .ToListAsync();

            return Ok(books);
        }

        [AllowAnonymous]
        [HttpGet("award-winners")]
        public async Task<IActionResult> GetAwardWinners()
        {
            var books = await _context.Books
                .Where(b => b.IsAwardWinner)
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .ToListAsync();

            return Ok(books);
        }
    }
}
