using System.Security.Cryptography;
using Bocchify_Api.Interfaces;

namespace Bocchify_Api.Services
{
    public class TokenService : ITokenService
    {
        public Task<string> GenerateAccessToken()
        {
            throw new NotImplementedException();
        }

        public Task<string> GeneratePasswordResetToken()
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateVerifyToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return await Task.FromResult(token);
        }
    }
}