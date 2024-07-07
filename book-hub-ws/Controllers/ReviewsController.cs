using book_hub_ws.DAL;
using book_hub_ws.Models;
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
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("concludeLoan")]
        public async Task<IActionResult> ConcludeLoan([FromBody] ReviewDto concludeLoanDto)
        {
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var loanRequest = await _context.LoanRequests
                .Include(lr => lr.Book)
                .Include(lr => lr.RequesterUser)
                .FirstOrDefaultAsync(lr => lr.Id == concludeLoanDto.LoanRequestId);

            if (loanRequest == null)
            {
                return NotFound("Loan request not found");
            }

            if (loanRequest.Book.UserId.ToString() != userId)
            {
                return Forbid("You are not authorized to conclude this loan");
            }

            loanRequest.Status = "concluded";
            loanRequest.EndDate = DateTime.UtcNow;

            var reciprocalLoanRequest = await _context.LoanRequests
                .Where(lr => lr.BookId == loanRequest.SpecificBookId && lr.RequesterUserId.ToString() == userId)
                .FirstOrDefaultAsync();

            if (reciprocalLoanRequest != null)
            {
                reciprocalLoanRequest.Status = "concluded";
                reciprocalLoanRequest.EndDate = DateTime.UtcNow;
            }

            var review = new Review
            {
                LoanRequestId = loanRequest.Id,
                ReviewerId = concludeLoanDto.ReviewerId,
                Rating = concludeLoanDto.Rating,
                Comment = concludeLoanDto.Comment
            };

            _context.Reviews.Add(review);

            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.BookId == loanRequest.BookId);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
            }

            var reciprocalLoan = await _context.Loans.FirstOrDefaultAsync(l => l.BookId == loanRequest.SpecificBookId);
            if (reciprocalLoan != null)
            {
                _context.Loans.Remove(reciprocalLoan);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("review")]
        public async Task<IActionResult> SubmitReview([FromBody] ReviewDto reviewDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            if (reviewDto.ReviewerId.ToString() != userId)
            {
                return Forbid("You are not authorized to submit this review");
            }

            var review = new Review
            {
                LoanRequestId = reviewDto.LoanRequestId,
                ReviewerId = reviewDto.ReviewerId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok();
        }



    }
}
