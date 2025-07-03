using Bocchify_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bocchify_Api.AppContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<VerifyToken> VerifyTokens { get; set; }
    }
}