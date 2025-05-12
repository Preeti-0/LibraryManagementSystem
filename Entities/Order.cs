namespace LibraryManagementSystem.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public bool IsCancelled { get; set; } = false;

        public string ClaimCode { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DiscountApplied { get; set; }

        public string? BillPath { get; set; }
        public bool IsFulfilled { get; set; } = false;
        public DateTime? FulfilledAt { get; set; }


        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
