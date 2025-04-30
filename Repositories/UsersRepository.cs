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

    public object FavouritesHandler(string mail_id, int userId)
    {
        Favourite ExistingFavourite = new Favourite { Mal_Id = mail_id, User_Id = userId };
        if (ExistingFavourite != null)
        {
            _context.Favourites.Remove(ExistingFavourite);
            return new { message = "removed from favourites" };
        }
        Favourite NewFavourite = new Favourite { Mal_Id = mail_id, User_Id = userId };
        _context.Favourites.Add(NewFavourite);
        return new { message = "added to favourites" };
    }

    // public bool RemoveUser(int userId)
    // {

    // }
}