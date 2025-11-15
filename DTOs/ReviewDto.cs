namespace BOSKOBACK.DTOs
{
    public class ReviewDto
    {
        public int? Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime? ReviewDate { get; set; }
        public string? ReviewerName { get; set; }
    }

    public class CreateReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
    }
}
