using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class PasswordController : ControllerBase
{
    private readonly UsersRepository _usersRepository;
    private readonly TokenService _tokenService;
    private readonly MailService _mailService;
    public PasswordController(UsersRepository userRepository, TokenService tokenService, MailService mailService)
    {
        _usersRepository = userRepository;
        _tokenService = tokenService;
        _mailService = mailService;
    }
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ChangePasswordDto dto)
    {
        User user = _usersRepository.GetUser(dto.Email);
        if (user == null)
        {
            return BadRequest(new { message = $"user with email: {dto.Email} was not found." });
        }
        if (user.PasswordResetTokenCreatedAt.HasValue &&
        DateTime.UtcNow - user.PasswordResetTokenCreatedAt.Value < TimeSpan.FromMinutes(30))
        {
            return BadRequest(new { message = "Please wait before requesting another reset email." });
        }
        string token = _tokenService.GeneratePasswordResetToken();
        _usersRepository.UpdateUser(user.Id, passwordResetToken: token);
        string frontendUrl = "http://localhost:4200/change-password";
        string resetLink = $"{frontendUrl}?token={token}&email={user.Email}";
        string subject = "Reset your password";
        string body = $"Click <a href='{resetLink}'>here</a> to reset your password.";
        await _mailService.SendEmailAsync(user.Email, subject, body);
        return Ok(new { message = "Reset password link sent to your email." });
    }

    [HttpPost("ResetPassword")]
    public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
    {
        User user = _usersRepository.GetUser(dto.Email);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid email." });
        }
        if (user.PasswordResetToken != dto.Token)
        {
            return BadRequest(new { message = "Invalid or expired token." });
        }
        if (!user.PasswordResetTokenCreatedAt.HasValue ||
        DateTime.UtcNow - user.PasswordResetTokenCreatedAt.Value > TimeSpan.FromMinutes(30))
        {
            return BadRequest(new { message = "Reset token has expired." });
        }

        User UpdatedUser = _usersRepository.UpdateUser(user.Id, password: dto.NewPassword);
        bool clearedPasswordResetToken = _usersRepository.clearPasswordResetToken(UpdatedUser);
        if (clearedPasswordResetToken == false)
        {
            return BadRequest(new { message = "User was not found." });
        }
        if (UpdatedUser == null)
        {
            return BadRequest(new { message = "Unexpected error." });
        }
        return Ok(new { message = "Password successfully updated." });
    }
}