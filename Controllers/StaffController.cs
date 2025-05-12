using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class StaffController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        // PUT: api/Staff/fulfill/{claimCode}
        [HttpPut("fulfill/{claimCode}")]
        public async Task<IActionResult> FulfillOrder(string claimCode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.ClaimCode == claimCode);

            if (order == null)
                return NotFound("Invalid Claim Code.");

            if (order.IsCancelled)
                return BadRequest("Order was cancelled.");

            if (order.IsFulfilled)
                return BadRequest("Order already fulfilled.");

            order.IsFulfilled = true;
            order.FulfilledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Order fulfilled successfully.");
        }
    }
}
