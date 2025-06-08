using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FavouritesRepository
{
    private readonly AppDbContext _context;

    public FavouritesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> FavouriteHandlerAsync(int mal_id, int user_id, string type)
    {
        var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == user_id);
        if (userExists == null)
        {
            return false;
        }

        var existingFavourite = await _context.Favourites
            .FirstOrDefaultAsync(f => f.Mal_Id == mal_id && f.UserId == user_id);

        if (existingFavourite != null)
        {
            _context.Favourites.Remove(existingFavourite);
            await _context.SaveChangesAsync();
            return false;
        }

        var newFavourite = new Favourite { Mal_Id = mal_id, UserId = user_id, Type = type };
        _context.Favourites.Add(newFavourite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Favourite>> GetFavouritesAsync(int user_id)
    {
        var userExists = await _context.Users
            .Include(u => u.Favourites)
            .FirstOrDefaultAsync(u => u.Id == user_id);

        if (userExists == null || userExists.Favourites == null || !userExists.Favourites.Any())
        {
            return null;
        }

        return userExists.Favourites.ToList();
    }

    public async Task<bool> IsFavouriteAsync(int mal_id, int user_id)
    {
        return await _context.Favourites
            .AnyAsync(f => f.Mal_Id == mal_id && f.UserId == user_id);
    }
}
