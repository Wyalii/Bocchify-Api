using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Column("id")]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Verified { get; set; }
    public string? ProfileImage { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenCreatedAt { get; set; }
    public ICollection<Favourite>? Favourites { get; set; }
}