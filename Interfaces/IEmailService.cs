namespace Bocchify_Api.Interfaces
{
    public interface IEmailService
    {
        public bool IsValidEmail(string email);
        Task SendVerificationEmail(string toEmail, string username, string verifyToken);
        Task SendPasswordResetEmail(string toEmail, string username, string resetToken);
    }
}