using System.Text;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_Secret"));

string allowedOrigin = Environment.GetEnvironmentVariable("Allowed_Origin");
string allowedOriginProd = Environment.GetEnvironmentVariable("Allowed_Origin_Prod");

var connectionString = Environment.GetEnvironmentVariable("Database_Connection_String");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigin", policy =>
    {
        policy.WithOrigins(allowedOrigin, allowedOriginProd)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
            ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<MailService>();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowedOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
