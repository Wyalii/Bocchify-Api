using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsersRepository _usersRepository;
    private readonly MailService _mailService;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;
    public AuthController(UsersRepository usersRepository, MailService mailService, PasswordService passwordService, TokenService tokenService)
    {
        _usersRepository = usersRepository;
        _mailService = mailService;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }
    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterUserDto registerUserDto)
    {
        try
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
            string verificationUrl = $"http://localhost:5227/api/auth/verify?email={registratedUser.Email}";
            _mailService.SendEmailAsync(registratedUser.Email, "Email Verification", $"<h1>Please verify your email by clicking the link below:</h1><a href='{verificationUrl}'");

            return Ok(new { message = $"User: {registratedUser.Username} has been registered." });
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginUserDto loginUserDto)
    {
        try
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
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("verify")]
    public IActionResult VerifyAccount([FromQuery] string email)
    {
        try
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
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }
}