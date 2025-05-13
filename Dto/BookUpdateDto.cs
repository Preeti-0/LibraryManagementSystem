using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LibraryManagementSystem.Dto
{
    public class BookUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string BookName { get; set; }

        public string Writer { get; set; }

        public string Genre { get; set; }

        public DateTime ReleaseDate { get; set; }

        public IFormFile? BookImage { get; set; }

        public decimal Price { get; set; }

        public string Language { get; set; }

        public string Format { get; set; }

        public int PublisherId { get; set; }

        public int Stock { get; set; }

        public string Description { get; set; }

        public bool IsOnSale { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public DateTime? DiscountStartDate { get; set; }

        public DateTime? DiscountEndDate { get; set; }

        public bool IsAvailableInLibrary { get; set; }
    }
}
