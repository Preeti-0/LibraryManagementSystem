using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Entities
{
    public class Member
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string MembershipId { get; set; } // For claim code validation

        [Required]
        public string Role { get; set; } = "Member"; // Could be: Member, Staff, Admin
        public bool EmailConfirmed { get; set; } = false;
        public string EmailConfirmationToken { get; set; } = Guid.NewGuid().ToString();

        public string? EmailVerificationToken { get; set; }
        public bool IsVerified { get; set; } = false;


        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; }
    }
}
