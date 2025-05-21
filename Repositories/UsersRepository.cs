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
    public User UpdateUser(
    int userId,
    string? username = null,
    string? email = null,
    string? password = null,
    string? profileImage = null,
    string? passwordResetToken = null
    )
    {
        try
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                user.Username = username;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                user.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                string hashedPassword = _passwordService.HashPassword(password);
                user.Password = hashedPassword;
            }

            if (!string.IsNullOrWhiteSpace(profileImage))
            {
                user.ProfileImage = profileImage;
            }
            if (!string.IsNullOrEmpty(passwordResetToken))
            {
                user.PasswordResetToken = passwordResetToken;
                user.PasswordResetTokenCreatedAt = DateTime.UtcNow;
            }

            _context.SaveChanges();
            return user;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public bool clearPasswordResetToken(User user)
    {
        User User = _context.Users.FirstOrDefault(u => u.Id == user.Id);
        if (User == null)
        {
            return false;
        }
        User.PasswordResetToken = null;
        User.PasswordResetTokenCreatedAt = null;
        _context.SaveChanges();
        return true;
    }


    // public bool RemoveUser(int userId)
    // {

    // }
}