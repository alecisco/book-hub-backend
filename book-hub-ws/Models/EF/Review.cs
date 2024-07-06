using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace book_hub_ws.Models.EF
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("LoanRequest")]
        public int LoanRequestId { get; set; }
        public LoanRequest LoanRequest { get; set; }

        [ForeignKey("User")]
        public int ReviewerId { get; set; }
        public User Reviewer { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

      public class ReviewDto
    {
        public int LoanRequestId { get; set; }
        public int ReviewerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
