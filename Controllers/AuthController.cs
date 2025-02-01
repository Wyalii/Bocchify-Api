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
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var ExistingUser = _context.Users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower());

        if (ExistingUser != null)
        {
            return BadRequest($"User With Provided Username: {request.Username}, already exists.");
        }
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        _context.Users.Add(new User { Username = request.Username, Password = passwordHash });
        _context.SaveChanges();
        return Ok($"Registered User: {request.Username}");
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest request)
    {

        return Ok();
    }
}