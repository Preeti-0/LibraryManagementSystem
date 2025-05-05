using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dto
{
    public class UserDto
    {
        [Key]

        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        //[Required]
        //public string PasswordHash { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
