using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace book_hub_ws.Models.EF
{
    public class LoanRequest
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [ForeignKey("User")]
        public int RequesterUserId { get; set; }
        public User RequesterUser { get; set; }

        public string Message { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public string Status { get; set; }

        [ForeignKey("SpecificBook")]
        public int? SpecificBookId { get; set; }
        public Book SpecificBook { get; set; }
    }

    public class LoanRequestDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int RequesterUserId { get; set; }
        public string RequesterUserName { get; set; }
        public string Message { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public int? SpecificBookId { get; set; }
        public string SpecificBookTitle { get; set; }
    }



    public class CreateLoanRequestDto
    {
        public int BookId { get; set; }
        public string Message { get; set; }

         public int? SpecificBookId { get; set; }
    }
    public class AcceptLoanRequestDto
    {
        public int? SpecificBookId { get; set; }
    }


}
