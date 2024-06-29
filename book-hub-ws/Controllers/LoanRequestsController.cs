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

        [HttpPost]
        public async Task<ActionResult<LoanRequestDto>> CreateLoanRequest(CreateLoanRequestDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var loanRequest = new LoanRequest
            {
                BookId = dto.BookId,
                RequesterUserId = int.Parse(userId),
                Message = dto.Message,
                RequestDate = DateTime.UtcNow,
                Status = "pending"
            };

            _context.LoanRequests.Add(loanRequest);
            await _context.SaveChangesAsync();

            var result = new LoanRequestDto
            {
                Id = loanRequest.Id,
                BookId = loanRequest.BookId,
                BookTitle = _context.Books.FirstOrDefault(b => b.BookId == dto.BookId)?.Title,
                RequesterUserId = loanRequest.RequesterUserId,
                RequesterUserName = _context.Users.FirstOrDefault(u => u.UserId == loanRequest.RequesterUserId)?.Name,
                Message = loanRequest.Message,
                RequestDate = loanRequest.RequestDate,
                Status = loanRequest.Status
            };

            return Ok(result);
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
                    Status = lr.Status
                })
                .ToListAsync();

            return Ok(loanRequests);
        }

        [HttpPut("{id}/accept")]
        public async Task<IActionResult> AcceptLoanRequest(int id)
        {
            var loanRequest = await _context.LoanRequests.FindAsync(id);
            if (loanRequest == null)
            {
                return NotFound("Loan request not found");
            }

            loanRequest.Status = "accepted";
            await _context.SaveChangesAsync();

            return NoContent();
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
