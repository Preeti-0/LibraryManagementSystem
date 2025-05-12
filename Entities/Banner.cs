namespace LibraryManagementSystem.Entities
{
    public class Banner
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
