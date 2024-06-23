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
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook([FromBody] BookCreateDto bookDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublicationYear = bookDto.PublicationYear,
                ISBN = bookDto.ISBN,
                Description = bookDto.Description,
                Condition = bookDto.Condition,
                PhotoUrl = bookDto.PhotoUrl,
                GenreId = bookDto.GenreId,
                UserId = int.Parse(userId)
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookToReturn = new BookDto
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                PublicationYear = book.PublicationYear,
                ISBN = book.ISBN,
                Description = book.Description,
                Condition = book.Condition,
                PhotoUrl = book.PhotoUrl,
                GenreId = book.GenreId,
                GenreName = (await _context.Genres.FindAsync(book.GenreId)).Name,
                UserId = book.UserId
            };

            return Ok(bookToReturn);
        }
    }
}
