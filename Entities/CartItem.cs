namespace LibraryManagementSystem.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

}
