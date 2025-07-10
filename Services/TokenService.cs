using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Bocchify_Api.Interfaces;
using Bocchify_Api.Models;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;

namespace Bocchify_Api.Services
{
    public class TokenService : ITokenService
    {
        public Task<string> GenerateAccessToken(User user)
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

            Console.WriteLine("JWT_SECRET: " + secret);
            Console.WriteLine("JWT_ISSUER: " + issuer);
            Console.WriteLine("JWT_AUDIENCE: " + audience);

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                 new Claim("nameid", user.Id.ToString()),
                 new Claim("email", user.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }


        public Task<string> GeneratePasswordResetToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            return Task.FromResult(token);
        }

        public Task<string> GenerateRefreshToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            return Task.FromResult(token);
        }

        public Task<string> GenerateVerifyToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            return Task.FromResult(token);
        }
    }
}