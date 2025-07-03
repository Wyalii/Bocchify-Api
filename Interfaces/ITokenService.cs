namespace Bocchify_Api.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateVerifyToken();
        Task<string> GenerateRefreshToken();
        Task<string> GenerateAccessToken();
        Task<string> GeneratePasswordResetToken();

    }
}