using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class PasswordController : ControllerBase
{
    private readonly UsersRepository _usersRepository;
    private readonly TokenService _tokenService;
    private readonly MailService _mailService;
    private readonly string frontendBaseUrl = "https://bocchify.netlify.app";
    public PasswordController(UsersRepository userRepository, TokenService tokenService, MailService mailService)
    {
        _usersRepository = userRepository;
        _tokenService = tokenService;
        _mailService = mailService;
    }
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ChangePasswordDto dto)
    {
        try
        {
            User user = await _usersRepository.GetUserAsync(dto.Email);
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
            await _usersRepository.UpdateUserAsync(user.Id, passwordResetToken: token);
            string frontendUrl = $"{frontendBaseUrl}/change-password";
            string resetLink = $"{frontendUrl}?token={token}&email={user.Email}";
            string subject = "Reset your password";
            string body = $"Click <a href='{resetLink}'>here</a> to reset your password.";
            await _mailService.SendEmailAsync(user.Email, subject, body);
            return Ok(new { message = "Reset password link sent to your email." });
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            User user = await _usersRepository.GetUserAsync(dto.Email);
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

            User UpdatedUser = await _usersRepository.UpdateUserAsync(user.Id, password: dto.NewPassword);
            bool clearedPasswordResetToken = await _usersRepository.ClearPasswordResetTokenAsync(UpdatedUser);
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}