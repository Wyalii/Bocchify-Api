public class Favourite
{
    public int Id { get; set; }
    public int Mal_Id { get; set; }
    public int User_Id { get; set; }
    public User User { get; set; }
}