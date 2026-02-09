using fitness_dash_api.Auth;
using fitness_dash_api.Context;
using fitness_dash_api.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fitness_dash_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {

        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || username == null)
            {
                return NotFound();
            }
            return Ok(new { Id = userId, Username = username });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var tokenStamp = User.Claims.FirstOrDefault(c => c.Type == "token_stamp")?.Value;

            if (int.TryParse(userId, out int id))
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    _context.SaveChanges();
                }
            }
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user != null && CheckPassword(request.Password, user.PasswordHash))
            {
                return Ok(GenerateJwtToken(user));
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (_context.Users.Any(x=>x.Username==request.Username))
            {
                return BadRequest("User already exists");
            }
            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Email = request.Email,
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(GenerateJwtToken(user));
        }

        private bool CheckPassword(string password, string passwordHash)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(null!, passwordHash, password);
            return result == PasswordVerificationResult.Success;
        }

        private string HashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException("password");

            var hasher = new PasswordHasher<User>();
            var hash = hasher.HashPassword(null!, password);

            return hash;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim("token_stamp", user.SecurityStamp)
            };
            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(8)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
