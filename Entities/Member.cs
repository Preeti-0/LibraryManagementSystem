namespace LibraryManagementSystem.Entities
{
    public class Member
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string MembershipId { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Bookmark> Bookmarks { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
