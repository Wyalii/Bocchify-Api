namespace Bocchify_Api.DTOS
{
    public class FavouriteDTO
    {
        public int Id { get; set; }
        public int Mal_Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}