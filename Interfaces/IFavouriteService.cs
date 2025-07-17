using Bocchify_Api.Contracts;

namespace Bocchify_Api.Interfaces
{
    public interface IFavouriteService
    {
        public Task<BaseResponse<object>> FavouriteHandler(FavouriteRequest favouriteRequest, int UserId);


    }
}