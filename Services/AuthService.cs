using System.Security.Claims;
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
            if (_context == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Database context (_context) is not initialized."
                };
            }

            if (_emailService == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Email service (_emailService) is not initialized."
                };
            }

            if (_passwordService == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Password service (_passwordService) is not initialized."
                };
            }

            if (_tokenService == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Success = false,
                    Message = "Token service (_tokenService) is not initialized."
                };
            }
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
            VerifyToken verifyToken = new VerifyToken
            {
                UserId = NewUser.Id,
                Token = VerifyToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };
            await _context.VerifyTokens.AddAsync(verifyToken);
            await _emailService.SendVerificationEmail(NewUser.Email, NewUser.Username, VerifyToken);
            await _context.SaveChangesAsync();

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
        public async Task<BaseResponse<UserDTO>> LogoutAsync(int UserId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                return new BaseResponse<UserDTO>()
                {
                    Success = false,
                    Message = "User Doesn't exists.",
                    Data = null
                };
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            _context.SaveChanges();
            UserDTO userDTO = new UserDTO()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Username,
                Avatar = user.Avatar
            };
            return new BaseResponse<UserDTO>()
            {
                Data = userDTO,
                Success = true,
                Message = $"User: {userDTO.Username} Logged Out."
            };
        }


        public async Task<BaseResponse<UserDTO>> VerifyUserAsync(VerifyUser VerifyRequest)
        {
            VerifyToken verifyToken = await _context.VerifyTokens.FirstOrDefaultAsync(vf => vf.Token == VerifyRequest.VerifyToken);
            if (verifyToken == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid Verify token."
                };
            }

            if (verifyToken.ExpiresAt <= DateTime.UtcNow)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "Expired verify token."
                };
            }

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == verifyToken.UserId);
            if (user == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "invalid user on verification."
                };
            }

            if (user.IsVerified)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "User is already verified."
                };
            }

            user.IsVerified = true;
            _context.VerifyTokens.Remove(verifyToken);
            UserDTO userDTO = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar
            };
            await _context.SaveChangesAsync();
            return new BaseResponse<UserDTO>
            {
                Data = userDTO,
                Success = true,
                Message = $"User: {userDTO.Username} verified!"
            };
        }
        public async Task<BaseResponse<UserDTO>> ChangePassword(ChangePassword ChangePasswordRequest)
        {
            PasswordResetToken passwordResetToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(prt => prt.Token == ChangePasswordRequest.Token);
            if (passwordResetToken == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid token."
                };
            }

            if (passwordResetToken.ExpiresAt <= DateTime.UtcNow)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "Expired token."
                };
            }

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == passwordResetToken.UserId);
            if (user == null)
            {

                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "user doesn't exists."
                };
            }

            bool IsPasswordValid = _passwordService.VerifyPassword(ChangePasswordRequest.CurrentPassword, user.PasswordHash);
            if (!IsPasswordValid)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = "incorrect current password."
                };
            }

            string NewHashPassword = _passwordService.HashPassword(ChangePasswordRequest.NewPassword);
            user.PasswordHash = NewHashPassword;
            _context.PasswordResetTokens.Remove(passwordResetToken);
            await _context.SaveChangesAsync();
            UserDTO userDTO = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar
            };
            return new BaseResponse<UserDTO>
            {
                Data = userDTO,
                Success = true,
                Message = "password changed succesfully!."
            };
        }

        public async Task<BaseResponse<UserDTO>> ForgotPassword(GenericEmail ForgotPasswordRequest)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == ForgotPasswordRequest.Email);
            if (user == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = $"user with email: {ForgotPasswordRequest.Email} was not found."
                };
            }

            PasswordResetToken passwordResetToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(prt => prt.UserId == user.Id);
            if (passwordResetToken != null)
            {
                if (passwordResetToken.ExpiresAt > DateTime.UtcNow)
                {

                    return new BaseResponse<UserDTO>
                    {
                        Data = null,
                        Success = false,
                        Message = "A password reset email was already sent and is still valid. Please check your inbox."
                    };
                }
                _context.PasswordResetTokens.Remove(passwordResetToken);
                _context.SaveChangesAsync();
            }

            string NewPasswordResetToken = await _tokenService.GeneratePasswordResetToken();
            PasswordResetToken NewPasswordResetTokenEntity = new PasswordResetToken
            {
                UserId = user.Id,
                Token = NewPasswordResetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow,
            };
            _context.PasswordResetTokens.Add(NewPasswordResetTokenEntity);
            _context.SaveChangesAsync();

            UserDTO userDTO = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar
            };
            await _emailService.SendPasswordResetEmail(user.Email, user.Username, NewPasswordResetTokenEntity.Token);
            return new BaseResponse<UserDTO>
            {
                Data = userDTO,
                Success = true,
                Message = "Forgot password reset is sent, please check your email!"
            };
        }

        public async Task<BaseResponse<UserDTO>> ResendVerifyToken(GenericEmail ResendVerifyUserRequest)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == ResendVerifyUserRequest.Email);
            if (user == null)
            {
                return new BaseResponse<UserDTO>
                {
                    Data = null,
                    Success = false,
                    Message = $"user with email: {ResendVerifyUserRequest.Email} was not found."
                };
            }

            VerifyToken verifyToken = await _context.VerifyTokens.FirstOrDefaultAsync(vt => vt.UserId == user.Id);
            if (verifyToken != null)
            {
                if (verifyToken.ExpiresAt > DateTime.UtcNow)
                {
                    return new BaseResponse<UserDTO>
                    {
                        Data = null,
                        Success = false,
                        Message = "A verification email was already sent and is still valid. Please check your inbox."
                    };
                }
                _context.VerifyTokens.Remove(verifyToken);
                _context.SaveChangesAsync();
            }

            string VerifyToken = await _tokenService.GenerateVerifyToken();
            VerifyToken NewVerifyToken = new VerifyToken
            {
                UserId = user.Id,
                Token = VerifyToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };
            await _context.VerifyTokens.AddAsync(NewVerifyToken);
            await _emailService.SendVerificationEmail(user.Email, user.Username, VerifyToken);
            await _context.SaveChangesAsync();

            UserDTO userDTO = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar
            };
            return new BaseResponse<UserDTO>
            {
                Data = userDTO,
                Success = true,
                Message = $"verification email was sent succesfully, check your email!"
            };

        }
    }
}