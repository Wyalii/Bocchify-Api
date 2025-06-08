using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsersRepository _usersRepository;
    private readonly MailService _mailService;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;
    private readonly string frontendBaseUrl = "https://bocchify.netlify.app";
    private readonly string backendBaseUrl = "https://bocchifyapi.onrender.com";
    public AuthController(UsersRepository usersRepository, MailService mailService, PasswordService passwordService, TokenService tokenService)
    {
        _usersRepository = usersRepository;
        _mailService = mailService;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        try
        {
            User registratedUser = await _usersRepository.CreateUserAsync(registerUserDto.Username, registerUserDto.Email, registerUserDto.Password, registerUserDto.ProfileImage);
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
            string verificationUrl = $"{backendBaseUrl}/api/auth/verify?email={registratedUser.Email}";
            _mailService.SendEmailAsync(registratedUser.Email, "Email Verification", $"<h1>Please verify your email by clicking the link below:</h1><a href='{verificationUrl}'");

            return Ok(new { message = $"User: {registratedUser.Username} has been registered." });
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        try
        {
            User foundUser = await _usersRepository.GetUserAsync(loginUserDto.Email);
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
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("verify")]
    public async Task<IActionResult> VerifyAccount([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is missing." });
            }

            User user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            if (user.Verified)
            {
                return Ok(new { message = "Account is already verified." });
            }

            await _usersRepository.UpdateUserVerificationStatusAsync(user.Email);

            return Redirect($"{frontendBaseUrl}/verified-success");
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }

    [HttpGet("ping")]
     public IActionResult Ping()
     {
        return Ok("Alive");
     }

}