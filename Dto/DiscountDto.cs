namespace LibraryManagementSystem.Dto
{
    public class DiscountDto
    {
        public bool IsOnSale { get; set; }

        public decimal DiscountPercentage { get; set; }

        public DateTime DiscountStartDate { get; set; }

        public DateTime DiscountEndDate { get; set; }
    }
}
