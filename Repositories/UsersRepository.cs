using MailKit;

public class UsersRepository
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;


    public UsersRepository(AppDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public User CreateUser(string username, string email, string password, string profileImage)
    {
        User ExistingUser = _context.Users.FirstOrDefault(u => u.Email == email);
        if (ExistingUser != null)
        {
            return null;
        }

        string hashedPassword = _passwordService.HashPassword(password);

        User NewUser = new User
        {
            Username = username,
            Email = email,
            Password = hashedPassword,
            Verified = false,
            ProfileImage = profileImage
        };

        _context.Users.Add(NewUser);
        _context.SaveChanges();
        return NewUser;
    }

    public User GetUser(string email)
    {
        User foundUser = _context.Users.FirstOrDefault(u => u.Email == email);
        if (foundUser == null)
        {
            return null;
        }

        return foundUser;
    }

    public bool UpdateUserVerificationStatus(string email)
    {
        User user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return false;

        user.Verified = true;
        _context.Users.Update(user);
        _context.SaveChanges();
        return true;
    }

    public object FavouriteHandler(int mal_id, int user_id)
    {
        User userExists = _context.Users.FirstOrDefault(u => u.Id == user_id);
        if (userExists == null)
        {
            return new { message = "invalid user id" };
        }
        Favourite existingFavourite = _context.Favourites.FirstOrDefault(f => f.Mal_Id == mal_id && f.UserId == user_id);
        if (existingFavourite != null)
        {
            _context.Favourites.Remove(existingFavourite);
            _context.SaveChanges();
            return new { message = "removed from favourites" };
        }
        Favourite NewFavourite = new Favourite { Mal_Id = mal_id, UserId = user_id };
        _context.Favourites.Add(NewFavourite);
        _context.SaveChanges();
        return new { message = "added to favourites" };
    }




    // public bool RemoveUser(int userId)
    // {

    // }
}