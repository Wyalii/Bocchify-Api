using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.Data;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public ActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.username) || string.IsNullOrEmpty(request.password))
        {
            return BadRequest(new { message = "Username and password are required." });
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

        return Ok();
    }
}