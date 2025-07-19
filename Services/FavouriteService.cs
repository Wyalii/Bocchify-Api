using AutoMapper;
using Bocchify_Api.AppContext;
using Bocchify_Api.Contracts;
using Bocchify_Api.DTOS;
using Bocchify_Api.Interfaces;
using Bocchify_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bocchify_Api.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public FavouriteService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BaseResponse<object>> FavouriteHandler(FavouriteRequest favouriteRequest, int UserId)
        {
            if (string.IsNullOrWhiteSpace(favouriteRequest.Type))
            {
                return new BaseResponse<object>()
                {
                    Success = false,
                    Message = "invalid type.",
                    Data = null
                };
            }
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

            Favourite ExistingFavourite = await _context.Favourites.FirstOrDefaultAsync(f => f.Mal_Id == favouriteRequest.Mal_Id && f.UserId == UserId);
            if (ExistingFavourite != null)
            {
                _context.Favourites.Remove(ExistingFavourite);
                await _context.SaveChangesAsync();
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
                UserId = UserId,
                Type = favouriteRequest.Type
            };

            _context.Favourites.AddAsync(NewFavourite);
            await _context.SaveChangesAsync();

            return new BaseResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Added To Favourites"
            };

        }

        public async Task<BaseResponse<object>> GetFavourites(int UserId)
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

            var favouriteEntities = await _context.Favourites.Where(f => f.UserId == UserId).ToListAsync();

            if (favouriteEntities == null || favouriteEntities.Count == 0)
            {
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = "No favourites.",
                    Data = null
                };
            }

            var favouriteDTOs = _mapper.Map<List<FavouriteDTO>>(favouriteEntities);

            return new BaseResponse<object>()
            {
                Success = true,
                Message = "succesfully fetched favourites.",
                Data = favouriteDTOs
            };
        }
    }
}