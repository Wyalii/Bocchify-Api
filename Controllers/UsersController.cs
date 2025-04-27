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
        User registratedUser = _usersRepository.CreateUser(registerUserDto.Username, registerUserDto.Email, registerUserDto.Password);
        if (registratedUser == null)
        {
            return BadRequest(new { message = "Problem On Registrating the user." });
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

        bool correctPassword = _passwordService.VerifyPassword(loginUserDto.Password, foundUser.Password);
        if (correctPassword == false)
        {
            return BadRequest(new { message = "Wrong assword try again." });
        }

        string token = _tokenService.GenerateToken(foundUser.Id.ToString(), foundUser.Email);

        return Ok(new { message = $"User: {foundUser.Username} has logged in!", newToken = token });
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

}