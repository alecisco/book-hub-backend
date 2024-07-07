using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace book_hub_ws.Models.EF
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public string LoanType { get; set; }

        public string? SpecificBookTitle { get; set; }

    }

    public class LoanDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string LoanType { get; set; }
        public string? SpecificBookTitle { get; set; }
    }

    public class LoanCreateDto
    {
        public int BookId { get; set; }
        public string LoanType { get; set; }
        public string? SpecificBookTitle { get; set; }
    }

    public class LoanedBookDto
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public bool Available { get; set; }
        public string PhotoUrl { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public string LoanType { get; set; }
        public string? SpecificBookTitle { get; set; }

        public bool PendingRequest { get; set; }

        public string LenderNickname { get; set; }
    }


    public class LoanHistoryDto
    {
        public int LoanRequestId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string BorrowerName { get; set; }
        public string LenderName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReviewHistoryDto BorrowerReview { get; set; }
        public ReviewHistoryDto LenderReview { get; set; }

    }

}
