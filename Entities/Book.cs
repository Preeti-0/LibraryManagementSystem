using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string BookName { get; set; }

        [Required]
        public string Writer { get; set; }

        public string Genre { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string? ISBN { get; set; }  // NEW

        public string? EditionType { get; set; }  // NEW

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string Language { get; set; }

        public string Format { get; set; }

        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public int Stock { get; set; }

        public string? Description { get; set; }

        public bool IsAvailableInLibrary { get; set; }  // NEW

        public bool IsOnSale { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public DateTime? DiscountStartDate { get; set; }

        public DateTime? DiscountEndDate { get; set; }

        public bool IsAwardWinner { get; set; }  // NEW

        public int SalesCount { get; set; } = 0;  // NEW

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // NEW

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }

}
