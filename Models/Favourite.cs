namespace Bocchify_Api.Models
{
    public class Favourite
    {
        public int Id { get; set; }
        public int Mal_Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}