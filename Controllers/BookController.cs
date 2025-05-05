using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Dto;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Reviews)
                .Select(b => new BookDto
                {
                    BookId = b.Id,
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            if (id != updatedBook.Id)
                return BadRequest();

            _context.Entry(updatedBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(b => b.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/book/5
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
