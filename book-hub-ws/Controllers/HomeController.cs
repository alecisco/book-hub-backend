using book_hub_ws.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace book_hub_ws.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("getHomeData")]
        public async Task<IActionResult> GetHomeData()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var user = await _context.Users
                .Where(u => u.UserId.ToString() == userId)
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Nickname,
                    Books = u.Books
                        .Select(b => new
                        {
                            b.BookId,
                            b.Title,
                            b.Author,
                            b.PublicationYear,
                            GenreName = b.Genre.Name,
                            b.PhotoUrl,
                            b.ISBN,
                            b.Description,
                            b.Condition,
                            OwnerName = (string?)null,
                            BorrowerName = _context.LoanRequests
                                .Where(lr => lr.BookId == b.BookId && lr.Status == "accepted" && lr.EndDate == null)
                                .Select(lr => lr.RequesterUser.Nickname)
                                .FirstOrDefault(),
                            Status = _context.LoanRequests
                                .Where(lr => lr.BookId == b.BookId && lr.Status == "accepted" && lr.EndDate == null)
                                .Select(lr => "prestato")
                                .FirstOrDefault() ??
                                (_context.Loans
                                    .Where(l => l.BookId == b.BookId)
                                    .Select(l => "disponibile per il prestito")
                                    .FirstOrDefault() ?? "in libreria"),
                            LoanRequestId = _context.LoanRequests
                                .Where(lr => lr.BookId == b.BookId && lr.Status == "accepted" && lr.EndDate == null)
                                .Select(lr => lr.Id)
                                .FirstOrDefault()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            var borrowedBooks = await _context.LoanRequests
                .Where(lr => lr.RequesterUserId.ToString() == userId && lr.Status == "accepted" && lr.EndDate == null)
                .Select(lr => new
                {
                    lr.Book.BookId,
                    lr.Book.Title,
                    lr.Book.Author,
                    lr.Book.PublicationYear,
                    GenreName = lr.Book.Genre.Name,
                    lr.Book.PhotoUrl,
                    lr.Book.ISBN,
                    lr.Book.Description,
                    lr.Book.Condition,
                    OwnerName = lr.Book.User.Nickname,
                    BorrowerName = (string?)null,
                    Status = "prestato",
                    LoanRequestId = lr.Id
                })
                .ToListAsync();

            user.Books.AddRange(borrowedBooks);

            var responseData = new { User = user };

            return Ok(responseData);
        }



    }
}
