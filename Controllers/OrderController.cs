using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using LibraryManagementSystem.Hubs;


namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IHubContext<OrderHub> _orderHub;

        public OrderController(ApplicationDbContext context, IEmailService emailService, IHubContext<OrderHub> orderHub)
        {
            _context = context;
            _emailService = emailService;
            _orderHub = orderHub;
        }

        // POST: api/Order/place
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder()
        {
            int memberId = int.Parse(User.FindFirst("userId").Value);

            var cartItems = await _context.CartItems
                .Include(c => c.Book)
                .Where(c => c.MemberId == memberId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Cart is empty.");

            // Calculate total price
            decimal subtotal = 0;
            foreach (var item in cartItems)
            {
                subtotal += item.Quantity * item.Book.Price;
            }

            // Apply discounts
            decimal discount = 0;

            if (cartItems.Sum(c => c.Quantity) >= 5)
                discount += 5;

            var successfulOrdersCount = await _context.Orders
                .CountAsync(o => o.MemberId == memberId && !o.IsCancelled);

            if (successfulOrdersCount >= 10)
                discount += 10;

            decimal discountAmount = (discount / 100) * subtotal;
            decimal total = subtotal - discountAmount;

            // Generate Claim Code
            var claimCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // Save order
            var order = new Order
            {
                MemberId = memberId,
                ClaimCode = claimCode,
                TotalAmount = total,
                DiscountApplied = discount,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Book.Price
                });

                // Optionally update book stock or sales
                item.Book.SalesCount += item.Quantity;
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems); // Clear cart
            await _context.SaveChangesAsync();

            var member = await _context.Members.FindAsync(memberId);

            // SignalR Broadcast: Order Placed
            await _orderHub.Clients.All.SendAsync("OrderPlaced", new
            {
                Message = $"{member.Name} just placed an order of {order.OrderItems.Count} book(s)!",
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });


            // Send email
            
            var emailBody = $@"
                <h3>Order Confirmation</h3>
                <p>Thank you for your order!</p>
                <p><strong>Claim Code:</strong> {claimCode}</p>
                <p><strong>Total:</strong> ₹{total}</p>
                <p>Show your claim code and membership ID at pickup.</p>";

            await _emailService.SendEmailAsync(member.Email, "Your Book Order Confirmation", emailBody);

            return Ok("Order placed successfully. Check your email for the claim code.");
        }

        // GET: api/Order/my-orders
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            int memberId = int.Parse(User.FindFirst("userId").Value);

            var orders = await _context.Orders
                .Where(o => o.MemberId == memberId)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Book)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.ClaimCode,
                    o.TotalAmount,
                    o.DiscountApplied,
                    o.IsCancelled,
                    o.IsFulfilled,  //  Add this
                    o.FulfilledAt,
                    Books = o.OrderItems.Select(i => new
                    {
                        i.BookId,
                        i.Book.BookName,
                        i.Quantity,
                        i.PriceAtPurchase
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }

        // PUT: api/Order/cancel/3
        [HttpPut("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            int memberId = int.Parse(User.FindFirst("userId").Value);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.MemberId == memberId);

            if (order == null)
                return NotFound("Order not found.");

            if (order.IsCancelled)
                return BadRequest("Order already cancelled.");

            order.IsCancelled = true;
            await _context.SaveChangesAsync();

            return Ok("Order cancelled successfully.");
        }
    }
}
