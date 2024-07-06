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
    public class LoanRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoanRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("createLoanRequest")]
        public async Task<ActionResult<LoanRequestDto>> CreateLoanRequest([FromBody] CreateLoanRequestDto createLoanRequestDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var loanRequest = new LoanRequest
            {
                BookId = createLoanRequestDto.BookId,
                RequesterUserId = int.Parse(userId),
                Message = createLoanRequestDto.Message,
                RequestDate = DateTime.UtcNow,
                Status = "pending",
                SpecificBookId = createLoanRequestDto.SpecificBookId
            };

            _context.LoanRequests.Add(loanRequest);
            await _context.SaveChangesAsync();

            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.BookId == createLoanRequestDto.BookId);

            return Ok(new LoanRequestDto
            {
                Id = loanRequest.Id,
                BookId = loanRequest.BookId,
                BookTitle = book.Title,
                RequesterUserId = loanRequest.RequesterUserId,
                Message = loanRequest.Message,
                RequestDate = loanRequest.RequestDate,
                Status = loanRequest.Status,
                SpecificBookId = loanRequest.SpecificBookId,
                SpecificBookTitle = loanRequest.SpecificBookId.HasValue ? _context.Books.Find(loanRequest.SpecificBookId.Value)?.Title : null
            });
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanRequestDto>>> GetLoanRequests()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var loanRequests = await _context.LoanRequests
                .Where(lr => lr.Book.UserId == int.Parse(userId) && lr.Status == "pending")
                .Select(lr => new LoanRequestDto
                {
                    Id = lr.Id,
                    BookId = lr.BookId,
                    BookTitle = lr.Book.Title,
                    RequesterUserId = lr.RequesterUserId,
                    RequesterUserName = lr.RequesterUser.Name,
                    Message = lr.Message,
                    RequestDate = lr.RequestDate,
                    Status = lr.Status,
                    SpecificBookId = lr.SpecificBookId,
                    SpecificBookTitle = lr.SpecificBook != null ? lr.SpecificBook.Title : null
                })
                .ToListAsync();

            return Ok(loanRequests);
        }


        [HttpPut("{id}/accept")]
        public async Task<IActionResult> AcceptLoanRequest(int id)
        {
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var loanRequest = await _context.LoanRequests
                .Include(lr => lr.Book)
                .Include(lr => lr.RequesterUser)
                .Include(lr => lr.SpecificBook)
                .FirstOrDefaultAsync(lr => lr.Id == id);

            if (loanRequest == null)
            {
                return NotFound("Loan request not found");
            }

            if (loanRequest.Book.UserId.ToString() != userId)
            {
                return Forbid("You are not authorized to accept this loan request");
            }

            loanRequest.Status = "accepted";
            loanRequest.EndDate = null;


            if (loanRequest.SpecificBookId.HasValue)
            {

                var loan = new Loan
                {
                    BookId = (int)loanRequest.SpecificBookId,
                    LoanType = loanRequest.SpecificBookId.HasValue ? "specificBook" : "loanOnly",
                    SpecificBookTitle = loanRequest.SpecificBookId.HasValue ? loanRequest.SpecificBook.Title : null
                };
                _context.Loans.Add(loan);

                var reciprocalLoanRequest = new LoanRequest
                {
                    BookId = loanRequest.SpecificBookId.Value,
                    RequesterUserId = loanRequest.Book.UserId,
                    Status = "accepted",
                    RequestDate = DateTime.UtcNow,
                    Message = "Reciprocal loan for " + loanRequest.Book.Title,
                    SpecificBookId = loanRequest.BookId,
                };
                _context.LoanRequests.Add(reciprocalLoanRequest);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }



        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectLoanRequest(int id)
        {
            var loanRequest = await _context.LoanRequests.FindAsync(id);
            if (loanRequest == null)
            {
                return NotFound("Loan request not found");
            }

            loanRequest.Status = "rejected";
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
