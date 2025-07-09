using Bocchify_Api.AppContext;
using Bocchify_Api.Contracts;
using Bocchify_Api.DTOS;
using Bocchify_Api.Interfaces;
using Bocchify_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bocchify_Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly PasswordService _passwordService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext context, ILogger<AuthService> logger, PasswordService passwordService, IEmailService emailService, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _passwordService = passwordService;
            _emailService = emailService;
            _tokenService = tokenService;
        }
        public async Task<BaseResponse<UserDTO>> RegisterAsync(RegisterUser RegisterRequest)
        {
            if (string.IsNullOrWhiteSpace(RegisterRequest.Username))
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Username field is invalid."
                };
            }

            if (RegisterRequest.Username.Length < 4)
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Username must contain more than 4 characters."
                };
            }

            if (string.IsNullOrWhiteSpace(RegisterRequest.Email))
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Email field is invalid."
                };
            }

            if (!_emailService.IsValidEmail(RegisterRequest.Email))
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Email format is invalid."
                };
            }

            if (string.IsNullOrWhiteSpace(RegisterRequest.Avatar))
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Avatar field is invalid."
                };
            }

            if (RegisterRequest.Password.Length < 5 && !RegisterRequest.Password.Any(char.IsDigit))
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Password must be at least 5 characters and contain at least one number."
                };
            }

            User AlreadyRegistered = await _context.Users.FirstOrDefaultAsync(u => u.Email == RegisterRequest.Email);
            if (AlreadyRegistered != null)
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Email is already used (registered).",
                };
            }

            User NewUser = new User()
            {
                Username = RegisterRequest.Username,
                Email = RegisterRequest.Email,
                Avatar = RegisterRequest.Avatar,
                PasswordHash = _passwordService.HashPassword(RegisterRequest.Password),
                RefreshToken = null,
                RefreshTokenExpiry = null,

            };

            UserDTO NewUserDto = new UserDTO()
            {
                Username = RegisterRequest.Username,
                Email = RegisterRequest.Email,
                Avatar = RegisterRequest.Avatar,

            };

            await _context.Users.AddAsync(NewUser);
            await _context.SaveChangesAsync();
            string VerifyToken = await _tokenService.GenerateVerifyToken();
            await _emailService.SendVerificationEmail(NewUser.Email, NewUser.Username, VerifyToken);
            return new BaseResponse<UserDTO>
            {
                Success = true,
                Message = $"User: {NewUser.Username} Registered Succesfully, Please check your Email!",
                Data = NewUserDto
            };
        }
        public async Task<BaseResponse<object>> LoginAsync(LoginUser LoginRequest)
        {
            if (!_emailService.IsValidEmail(LoginRequest.Email))
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = "Email format is invalid."
                };
            }

            if (string.IsNullOrWhiteSpace(LoginRequest.Password))
            {

                return new BaseResponse<object>
                {
                    Success = false,
                    Message = "Password format is invalid."
                };
            }

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == LoginRequest.Email);
            if (user == null)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = "user with provided email is not registered."
                };
            }

            bool CorrectPassword = _passwordService.VerifyPassword(LoginRequest.Password, user.PasswordHash);
            if (!CorrectPassword)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = "incorrect password."
                };
            }
            string AccessToken = await _tokenService.GenerateAccessToken(user);

            if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                string RefreshToken = await _tokenService.GenerateRefreshToken();
                user.RefreshToken = RefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            }

            await _context.SaveChangesAsync();

            UserDTO userDto = new UserDTO()
            {
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,

            };

            var responseData = new
            {
                User = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                },
                accessToken = AccessToken,

            };

            return new BaseResponse<object>
            {
                Success = true,
                Message = "Login successful.",
                Data = responseData
            };

        }
        public Task<BaseResponse<UserDTO>> LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<UserDTO>> VerifyUserAsync(GenericEmail VerifyRequest)
        {
            throw new NotImplementedException();
        }
        public Task<BaseResponse<UserDTO>> ChangePassword(ChangePassword ChangePasswordRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<UserDTO>> ForgotPassword(GenericEmail ForgotPasswordRequest)
        {
            throw new NotImplementedException();
        }


    }
}