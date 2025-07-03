using Bocchify_Api.Contracts;
using Bocchify_Api.DTOS;


namespace Bocchify_Api.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<UserDTO>> RegisterAsync(RegisterUser RegisterRequest);
        Task<BaseResponse<UserDTO>> LoginAsync(LoginUser LoginRequest);
        Task<BaseResponse<UserDTO>> LogoutAsync();
        Task<BaseResponse<UserDTO>> VerifyUserAsync(GenericEmail VerifyRequest);
        Task<BaseResponse<UserDTO>> ChangePassword(ChangePassword ChangePasswordRequest);
        Task<BaseResponse<UserDTO>> ForgotPassword(GenericEmail ForgotPasswordRequest);
    }
}