public class Favourite
{
    public int Id { get; set; }
    public string Mal_Id { get; set; }
    public int User_Id { get; set; }
    public User User { get; set; }
}