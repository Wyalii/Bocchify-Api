using Bocchify_Api.Contracts;
using Bocchify_Api.DTOS;


namespace Bocchify_Api.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<UserDTO>> RegisterAsync(RegisterUser RegisterRequest);
        Task<BaseResponse<object>> LoginAsync(LoginUser LoginRequest);
        Task<BaseResponse<UserDTO>> LogoutAsync(int UserId);
        Task<BaseResponse<UserDTO>> VerifyUserAsync(VerifyUser VerifyUser);
        Task<BaseResponse<UserDTO>> ChangePassword(ChangePassword ChangePasswordRequest);
        Task<BaseResponse<UserDTO>> ForgotPassword(GenericEmail ForgotPasswordRequest);
    }
}