using Microsoft.AspNetCore.Mvc;
using book_hub_ws.Models.EF;
using book_hub_ws.DAL;
using book_hub_ws.Utils;
using Microsoft.EntityFrameworkCore;
using book_hub_ws.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;

namespace book_hub_ws.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AccountController(AppDbContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == request.Nickname);
            if (existingUser != null)
            {
                ModelState.AddModelError("Nickname", "Nickname già in uso, utilizzare un altro nickname.");
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Nickname = request.Nickname,
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = CryptoUtils.HashPassword(request.Password),
                Community = request.Community
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (request.FavoriteGenres != null && request.FavoriteGenres.Count > 0)
            {
                var userGenres = request.FavoriteGenres.Select(genreId => new UserGenre
                {
                    UserId = user.UserId,
                    GenreId = genreId
                }).ToList();

                _context.UserGenres.AddRange(userGenres);
                await _context.SaveChangesAsync();
            }

            var jwt = JwtUtils.GenerateToken(_jwtSettings, user);
            return Ok(new { Token = jwt });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !CryptoUtils.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var jwt = JwtUtils.GenerateToken(_jwtSettings, user);
            return Ok(new { Token = jwt });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.UserGenres)
                .ThenInclude(ug => ug.Genre)
                .FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));

            if (user == null)
            {
                return NotFound("User not found");
            }

            var profile = new ProfileUpdateRequest
            {
                Nickname = user.Nickname,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Community = user.Community,
                FavoriteGenres = user.UserGenres.Select(ug => ug.GenreId).ToList()  
            };

            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var user = await _context.Users
                .Include(u => u.UserGenres)
                .FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Nickname = request.Nickname;
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Community = request.Community;

            user.UserGenres.Clear();
            foreach (var genreId in request.FavoriteGenres)
            {
                user.UserGenres.Add(new UserGenre { UserId = user.UserId, GenreId = genreId });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!CryptoUtils.VerifyPassword(request.OldPassword, user.PasswordHash))
            {
                return BadRequest("Old password is incorrect");
            }

            user.PasswordHash = CryptoUtils.HashPassword(request.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
