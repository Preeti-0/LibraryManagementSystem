using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Dto;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublisherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/publisher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers()
        {
            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .Select(p => new PublisherDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    BookCount = p.Books.Count
                })
                .ToListAsync();

            return Ok(publishers);
        }


        // GET: api/publisher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (publisher == null)
                return NotFound();

            return publisher;
        }

        // POST: api/publisher
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Publisher>> CreatePublisher(PublisherCreateDto dto)
        {
            var publisher = new Publisher
            {
                Name = dto.Name,
                Address = dto.Address
            };

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPublisher), new { id = publisher.Id }, publisher);
        }


        // PUT: api/publisher/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, Publisher updated)
        {
            if (id != updated.Id)
                return BadRequest();

            var existing = await _context.Publishers.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = updated.Name;
            existing.Address = updated.Address;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/publisher/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
                return NotFound();

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
