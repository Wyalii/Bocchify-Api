using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class UsersRepository
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;

    public UsersRepository(AppDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<User?> CreateUserAsync(string username, string email, string password, string profileImage)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return null;
        }

        var hashedPassword = _passwordService.HashPassword(password);

        var newUser = new User
        {
            Username = username,
            Email = email,
            Password = hashedPassword,
            Verified = false,
            ProfileImage = profileImage
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }

    public async Task<User?> GetUserAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UpdateUserVerificationStatusAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;

        user.Verified = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> UpdateUserAsync(
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(username))
                user.Username = username;

            if (!string.IsNullOrWhiteSpace(email))
                user.Email = email;

            if (!string.IsNullOrWhiteSpace(password))
                user.Password = _passwordService.HashPassword(password);

            if (!string.IsNullOrWhiteSpace(profileImage))
                user.ProfileImage = profileImage;

            if (!string.IsNullOrEmpty(passwordResetToken))
            {
                user.PasswordResetToken = passwordResetToken;
                user.PasswordResetTokenCreatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return user;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ClearPasswordResetTokenAsync(User user)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser == null)
        {
            return false;
        }

        existingUser.PasswordResetToken = null;
        existingUser.PasswordResetTokenCreatedAt = null;
        await _context.SaveChangesAsync();
        return true;
    }
}
