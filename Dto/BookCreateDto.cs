using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LibraryManagementSystem.Dto
{
    public class BookCreateDto
    {
        [Required]
        public string BookName { get; set; }

        [Required]
        public string Writer { get; set; }

        [Required]
        public string Genre { get; set; }

        public DateTime ReleaseDate { get; set; }

        public IFormFile? BookImage { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Format { get; set; }

        [Required]
        public int PublisherId { get; set; }

        [Required]
        public int Stock { get; set; }

        public string? Description { get; set; }

        // ✅ NEW Optional Fields
        public string? ISBN { get; set; }

        public string? EditionType { get; set; }

        public bool IsAvailableInLibrary { get; set; } = false;
    }
}
