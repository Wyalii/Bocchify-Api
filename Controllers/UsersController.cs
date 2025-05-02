
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersRepository _usersRepository;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;
    private readonly MailService _mailService;
    public UsersController(UsersRepository userRepository, PasswordService passwordService, TokenService tokenService, MailService mailService)
    {
        _usersRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _mailService = mailService;
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterUserDto registerUserDto)
    {
        User registratedUser = _usersRepository.CreateUser(registerUserDto.Username, registerUserDto.Email, registerUserDto.Password, registerUserDto.ProfileImage);
        if (registratedUser == null)
        {
            return BadRequest(new { message = "Problem On Registrating the user." });
        }
        if (String.IsNullOrWhiteSpace(registerUserDto.Username))
        {
            return BadRequest(new { message = "Invalid Username Input." });
        }
        if (String.IsNullOrWhiteSpace(registerUserDto.Password))
        {
            return BadRequest(new { message = "Invalid Password Input." });
        }
        if (String.IsNullOrEmpty(registerUserDto.ProfileImage))
        {
            return BadRequest(new { message = "profile image is null." });
        }
        string verificationUrl = $"http://localhost:5227/api/users/verify?email={registratedUser.Email}";
        _mailService.SendEmailAsync(registratedUser.Email, "Email Verification", $"<h1>Please verify your email by clicking the link below:</h1><a href='{verificationUrl}'");

        return Ok(new { message = $"User: {registratedUser.Username} has been registered." });
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginUserDto loginUserDto)
    {
        User foundUser = _usersRepository.GetUser(loginUserDto.Email);
        if (foundUser == null)
        {
            return BadRequest(new { message = $"user with email: {loginUserDto.Email} not found." });
        }
        if (foundUser.Verified == false)
        {
            return BadRequest(new { message = "please check your email and verify your account first." });
        }

        bool correctPassword = _passwordService.VerifyPassword(loginUserDto.Password, foundUser.Password);
        if (correctPassword == false)
        {
            return BadRequest(new { message = "Wrong password try again." });
        }

        string token = _tokenService.GenerateToken(foundUser.Id.ToString(), foundUser.Email);

        return Ok(new { Name = foundUser.Username, message = $"User: {foundUser.Username} has logged in!", newToken = token, ProfileImage = foundUser.ProfileImage });
    }

    [HttpGet("verify")]
    public IActionResult VerifyAccount([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new { message = "Email is missing." });
        }

        User user = _usersRepository.GetUser(email);
        if (user == null)
        {
            return BadRequest(new { message = "User not found." });
        }

        if (user.Verified)
        {
            return Ok(new { message = "Account is already verified." });
        }

        _usersRepository.UpdateUserVerificationStatus(user.Email);

        return Redirect("http://localhost:4200/verified-success");

    }

    [HttpPost("Favourite")]
    public IActionResult Favourite([FromBody] FavouriteRequest request)
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Missing or invalid Authorization header.");
        }
        var token = authHeader.Substring("Bearer ".Length).Trim();
        var claims = _tokenService.GetClaimsFromToken(token);
        if (claims == null)
        {
            return Unauthorized("Invalid or expired token.");
        }

        var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }

        int user_id = int.Parse(userIdClaim.Value);

        bool response = _usersRepository.FavouriteHandler(request.MalId, user_id);
        return Ok(response);
    }
    [HttpPost("CheckFavourite")]
    public IActionResult CheckFavourite([FromBody] FavouriteRequest request)
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Missing or invalid Authorization header.");
        }
        var token = authHeader.Substring("Bearer ".Length).Trim();
        var claims = _tokenService.GetClaimsFromToken(token);
        if (claims == null)
        {
            return Unauthorized("Invalid or expired token.");
        }

        var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }

        int user_id = int.Parse(userIdClaim.Value);
        bool isFavourited = _usersRepository.IsFavourite(request.MalId, user_id);
        return Ok(new { isFavourited });

    }
}