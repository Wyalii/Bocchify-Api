public class UsersRepository
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;

    public UsersRepository(AppDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public User CreateUser(string username, string email, string password)
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
            Password = hashedPassword
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

    // public bool UpdateUser(int userId)
    // {

    // }

    // public bool RemoveUser(int userId)
    // {

    // }
}