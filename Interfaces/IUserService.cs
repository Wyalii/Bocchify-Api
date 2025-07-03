using Bocchify_Api.Contracts;
using Bocchify_Api.DTOS;

namespace Bocchify_Api.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserDTO>> GetUserProfileAsync();
        Task<BaseResponse<FavouriteDTO>> GetUserFavouritesAsync();
        Task<BaseResponse<UserDTO>> UpdateUserProfileAsync(UpdateProfile UpdateProfileRequest);
    }
}