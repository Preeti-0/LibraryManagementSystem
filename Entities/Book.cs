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

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string Language { get; set; }

        public string Format { get; set; }

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        public int Stock { get; set; }

        public string Description { get; set; }

        public bool IsOnSale { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public DateTime? DiscountStartDate { get; set; }

        public DateTime? DiscountEndDate { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
