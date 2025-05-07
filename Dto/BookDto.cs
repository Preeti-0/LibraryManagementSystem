using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dto
{
    public class BookDto
    {
        public int Id { get; set; }  // ✅ Use Id, not BookId

        public string BookName { get; set; }

        public string Writer { get; set; }

        public string Genre { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string PhotoPath { get; set; }

        public decimal Price { get; set; }

        public string Language { get; set; }

        public string Format { get; set; }

        public string PublisherName { get; set; }

        public double AverageRating { get; set; }

        public bool IsOnSale { get; set; }

        public int Stock { get; set; }
    }
}
