using System.Net.Mail;
using Bocchify_Api.Interfaces;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using dotenv.net;


namespace Bocchify_Api.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendVerificationEmail(string toEmail, string username, string verifyToken)
        {
            DotEnv.Load();
            var verifyUrl = $"https://yourfrontend.com/verify?token={verifyToken}";
            var smtpPass = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASS");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bocchify", _config["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Verify Your Email - Bocchify";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <h2>Hello, {username}!</h2>
                <p>Click the link below to verify your email:</p>
                <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>",
                TextBody = $"Hello {username},\n\nPlease verify your email by visiting the following link:\n{verifyUrl}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config["Email:SmtpUser"], smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }


        public async Task SendPasswordResetEmail(string toEmail, string username, string resetToken)
        {
            var resetUrl = $"https://yourfrontend.com/reset-password?token={resetToken}";
            var smtpPass = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASS");

            if (string.IsNullOrEmpty(smtpPass))
                throw new Exception("SMTP password not found in environment variables.");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bocchify", _config["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Reset Your Password - Bocchify";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
            <h2>Hello, {username}!</h2>
            <p>You requested to reset your password.</p>
            <p>Click the link below to continue:</p>
            <p><a href=""{resetUrl}"">{resetUrl}</a></p>
            <p>If you didn’t request this, you can ignore this email.</p>",
                TextBody = $"Hello {username},\n\nYou requested to reset your password. Click the link below to proceed:\n{resetUrl}\n\nIf you didn't request this, you can safely ignore this email."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["Email:SmtpUser"], smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}