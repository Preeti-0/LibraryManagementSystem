using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dto
{
    public class CommentCreateDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; }

    }
}
