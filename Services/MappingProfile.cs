using AutoMapper;
using Bocchify_Api.DTOS;
using Bocchify_Api.Models;

namespace Bocchify_Api.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Favourite, FavouriteDTO>();
            CreateMap<User, UserDTO>();
        }
    }
}