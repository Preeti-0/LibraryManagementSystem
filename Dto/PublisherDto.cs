namespace LibraryManagementSystem.Dto
{
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int BookCount { get; set; }  // Optional
    }
}
