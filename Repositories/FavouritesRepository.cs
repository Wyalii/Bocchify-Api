using Microsoft.EntityFrameworkCore;

public class FavouritesRepository
{
    private readonly AppDbContext _context;
    public FavouritesRepository(AppDbContext context)
    {
        _context = context;
    }
    public bool FavouriteHandler(int mal_id, int user_id, string type)
    {
        User userExists = _context.Users.FirstOrDefault(u => u.Id == user_id);
        if (userExists == null)
        {
            return false;
        }
        Favourite existingFavourite = _context.Favourites.FirstOrDefault(f => f.Mal_Id == mal_id && f.UserId == user_id);
        if (existingFavourite != null)
        {
            _context.Favourites.Remove(existingFavourite);
            _context.SaveChanges();
            return false;
        }
        Favourite NewFavourite = new Favourite { Mal_Id = mal_id, UserId = user_id, Type = type };
        _context.Favourites.Add(NewFavourite);
        _context.SaveChanges();
        return true;
    }
    public List<Favourite> GetFavourites(int user_id)
    {
        User UserExists = _context.Users.Include(u => u.Favourites).FirstOrDefault(u => u.Id == user_id);
        if (UserExists == null)
        {
            return null;
        }
        List<Favourite> UserFavourites = UserExists.Favourites.ToList();
        if (UserFavourites.Count == 0)
        {
            return null;
        }
        return UserFavourites;
    }
    public bool IsFavourite(int mal_id, int user_id)
    {
        return _context.Favourites.Any(f => f.Mal_Id == mal_id && f.UserId == user_id);
    }
}