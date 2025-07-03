using System.Text;
using Bocchify_Api.AppContext;
using Bocchify_Api.Interfaces;
using Bocchify_Api.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bocchify_Api.Extensions
{
    public static class ServiceExtensions
    {

        public static void ConfigureApplicationBuilder(this IHostApplicationBuilder builder)
        {
            DotEnv.Load();
            string ConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")!;
            string Jwt_Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
            string Jwt_Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
            string Jwt_Secret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
            if (ConnectionString == null || Jwt_Audience == null || Jwt_Issuer == null || Jwt_Secret == null)
            {
                throw new Exception("Missing required environment variables.");
            }
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(ConnectionString));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = Jwt_Issuer,
                      ValidAudience = Jwt_Audience,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt_Secret))
                  };
              });
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<PasswordService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Bocchify API", Version = "v1" });
            });
        }
    }
}