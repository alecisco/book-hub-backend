using book_hub_ws.DAL;
using book_hub_ws.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace book_hub_ws.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class LoansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoansController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("createLoan")]
        public async Task<ActionResult<LoanDto>> CreateLoan(LoanCreateDto loanCreateDto)
        {
            var loan = new Loan
            {
                BookId = loanCreateDto.BookId,
                LoanType = loanCreateDto.LoanType,
                SpecificBookTitle = loanCreateDto.SpecificBookTitle
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("loanedBooks")]
        public async Task<ActionResult<IEnumerable<LoanedBookDto>>> GetLoanedBooks()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var loanedBooks = await _context.Loans
                .Include(l => l.Book)
                .ThenInclude(b => b.Genre)
                .Where(l => l.Book.UserId != int.Parse(userId))
                .Where(l => !_context.LoanRequests.Any(lr => lr.BookId == l.BookId && lr.Status == "accepted" && lr.EndDate == null))
                .Select(l => new LoanedBookDto
                {
                    LoanId = l.Id,
                    BookId = l.BookId,
                    Title = l.Book.Title,
                    Author = l.Book.Author,
                    PublicationYear = l.Book.PublicationYear,
                    ISBN = l.Book.ISBN,
                    Description = l.Book.Description,
                    Condition = l.Book.Condition,
                    PhotoUrl = l.Book.PhotoUrl,
                    GenreId = l.Book.GenreId,
                    GenreName = l.Book.Genre.Name,
                    LoanType = l.LoanType,
                    SpecificBookTitle = l.SpecificBookTitle
                })
                .ToListAsync();

            return Ok(loanedBooks);
        }


        [HttpDelete("retractLoan/{bookId}")]
        public async Task<IActionResult> RetractLoan(int bookId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var loan = await _context.Loans
                                     .Where(l => l.BookId == bookId && l.Book.UserId.ToString() == userId)
                                     .FirstOrDefaultAsync();

            if (loan == null)
            {
                return NotFound("Loan not found or you are not the owner of the book");
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}
