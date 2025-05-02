using System.ComponentModel.DataAnnotations.Schema;

public class Favourite
{
    [Column("id")]
    public int Id { get; set; }
    public int Mal_Id { get; set; }
    public int UserId { get; set; }
}