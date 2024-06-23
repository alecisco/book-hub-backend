using book_hub_ws.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                                         Books = u.Books.Select(b => new
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
                                             Status = _context.Loans.Where(l => l.BookId == b.BookId)
                                                                     .Select(l => l.BorrowerUserId.HasValue ? "prestato" : "disponibile per il prestito")
                                                                     .FirstOrDefault() ?? "in libreria"
                                         })
                                     })
                                     .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }

            var responseData = new { User = user };

            return Ok(responseData);
        }

    }
}
