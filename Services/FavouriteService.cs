using Bocchify_Api.AppContext;
using Bocchify_Api.Contracts;
using Bocchify_Api.Interfaces;
using Bocchify_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bocchify_Api.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly AppDbContext _context;
        public FavouriteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse<object>> FavouriteHandler(FavouriteRequest favouriteRequest, int UserId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                return new BaseResponse<object>()
                {
                    Success = false,
                    Message = "User Doesn't exists.",
                    Data = null
                };
            }

            Favourite ExistingFavourite = await _context.Favourites.FirstOrDefaultAsync(f => f.Id == favouriteRequest.Mal_Id);
            if (ExistingFavourite != null)
            {
                _context.Favourites.Remove(ExistingFavourite);
                _context.SaveChangesAsync();
                return new BaseResponse<object>
                {
                    Success = true,
                    Message = "Removed From Favourites.",
                    Data = null
                };
            }

            Favourite NewFavourite = new Favourite
            {
                Mal_Id = favouriteRequest.Mal_Id,
                UserId = UserId
            };

            _context.Favourites.AddAsync(NewFavourite);
            _context.SaveChangesAsync();

            return new BaseResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Added To Favourites"
            };

        }
    }
}