using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dto
{
    public class PublisherCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
