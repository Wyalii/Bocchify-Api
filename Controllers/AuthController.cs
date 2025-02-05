using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.Data;
using dotenv.net;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private string JwtKey = Environment.GetEnvironmentVariable("JWT_Secret");
    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public ActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
        {
            return BadRequest(new { message = "invalid username or password input." });
        }

        var ExistingUser = _context.Users.FirstOrDefault(u => u.Username.ToLower() == request.username.ToLower());

        if (ExistingUser != null)
        {
            return BadRequest(new { message = $"User With Provided Username: {request.username}, already exists." });
        }
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.password);

        _context.Users.Add(new User { Username = request.username, Password = passwordHash });
        _context.SaveChanges();
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
        {
            return BadRequest(new { message = $"invalid username or password input." });
        }

        var ExistingUser = _context.Users.FirstOrDefault(u => u.Username.ToLower() == request.username.ToLower());
        if (ExistingUser == null)
        {
            return BadRequest(new { message = $"user with provided username: {request.username}, doesn't exists." });
        }

        bool CorrectPassword = BCrypt.Net.BCrypt.Verify(request.password, ExistingUser.Password);
        if (CorrectPassword == false)
        {
            return BadRequest(new { message = $"incorrect password." });
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Claim[] claims =
        {
          new Claim( "id",ExistingUser.Id.ToString()),
          new Claim("username",ExistingUser.Username),
          new Claim("role","User")
        };

        var token = new JwtSecurityToken(
          claims: claims,
          expires: DateTime.UtcNow.AddHours(1),
          signingCredentials: creds
        );

        return Ok(new { message = $"User {ExistingUser.Username} has logged in.", token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}