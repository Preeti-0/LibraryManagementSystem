namespace LibraryManagementSystem.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; }

        public DateTime OrderDate { get; set; }

        public bool IsCancelled { get; set; }

        public string ClaimCode { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DiscountApplied { get; set; }

        public string BillPath { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
