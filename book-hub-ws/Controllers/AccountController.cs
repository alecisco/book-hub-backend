using Microsoft.AspNetCore.Mvc;
using book_hub_ws.Models.EF;
using book_hub_ws.DAL;
using book_hub_ws.Utils;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using book_hub_ws.Models;

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


    }
}
