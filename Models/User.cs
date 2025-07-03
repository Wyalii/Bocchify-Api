namespace Bocchify_Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiry { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public List<Favourite> Favourites { get; set; } = new List<Favourite>();
    }
}