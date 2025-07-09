using Bocchify_Api.Models;

namespace Bocchify_Api.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateVerifyToken();
        Task<string> GenerateRefreshToken();
        Task<string> GenerateAccessToken(User user);
        Task<string> GeneratePasswordResetToken();

    }
}