
namespace Bocchify_Api.DTOS
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public List<FavouriteDTO> Favourites { get; set; } = new List<FavouriteDTO>();
    }
}