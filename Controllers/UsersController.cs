using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersRepository _usersRepository;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;
    public UsersController(UsersRepository userRepository, PasswordService passwordService, TokenService tokenService)
    {
        _usersRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterUserDto registerUserDto)
    {
        User registratedUser = _usersRepository.CreateUser(registerUserDto.Username, registerUserDto.Email, registerUserDto.Password);
        if (registratedUser == null)
        {
            return BadRequest(new { message = "Problem On Registrating the user." });
        }

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
}