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

            User NewUser = new User()
            {
                Username = RegisterRequest.Username,
                Email = RegisterRequest.Email,
                Avatar = RegisterRequest.Avatar,
                PasswordHash = _passwordService.HashPassword(RegisterRequest.Password),
                RefreshToken = null,
                RefreshTokenExpiry = null,
                Favourites = null
            };

            UserDTO NewUserDto = new UserDTO()
            {
                Username = RegisterRequest.Username,
                Email = RegisterRequest.Email,
                Avatar = RegisterRequest.Avatar,
                Favourites = null
            };

            User AlreadyRegistered = await _context.Users.FirstOrDefaultAsync(u => u.Email == RegisterRequest.Email);
            if (AlreadyRegistered != null)
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Email is already used (registered).",
                };
            }

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
        public Task<BaseResponse<UserDTO>> LoginAsync(LoginUser LoginRequest)
        {
            throw new NotImplementedException();
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