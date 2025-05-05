namespace LibraryManagementSystem.Dto
{
    public class CommentDto
    {
        public int Id { get; set; }

        public string MemberName { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; }
    }
}
