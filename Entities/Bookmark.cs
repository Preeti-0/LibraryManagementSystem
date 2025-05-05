namespace LibraryManagementSystem.Entities
{
    public class Bookmark
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime BookmarkedOn { get; set; }
    }
}
