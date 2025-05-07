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
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    BookName = b.BookName,
                    Writer = b.Writer,
                    Genre = b.Genre,
                    ReleaseDate = b.ReleaseDate,
                    PhotoPath = b.ImageUrl,
                    Price = b.Price,
                    Language = b.Language,
                    Format = b.Format,
                    PublisherName = b.Publisher.Name,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
                    IsOnSale = b.IsOnSale,
                    Stock = b.Stock
                })
                .ToListAsync();

            return Ok(books);
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
        // Only Admins can CREATE books
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromForm] BookCreateDto dto)
        {
            // ✅ Simulate image upload (you can later add real Cloud or disk upload)
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
                IsOnSale = false // default
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }


        // PUT: api/book/5
        // Only Admins can UPDATE books
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched book ID.");

            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound("Book not found.");

            // 📷 Handle optional image update
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

            // 📝 Update other fields
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

            await _context.SaveChangesAsync();

            //return NoContent();
            return Ok(book);
        }


        // DELETE: api/book/5
        // ✅ Only Admins can DELETE books
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
    }
}
