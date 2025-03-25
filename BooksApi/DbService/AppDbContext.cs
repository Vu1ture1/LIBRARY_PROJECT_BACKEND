using BooksApi.Entities;
using BooksApi.Entities.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BooksApi.DbService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<User> UsersTab { get; set; }
        public DbSet<Author> AuthorsTab { get; set; }
        public DbSet<Book> BooksTab { get; set; }
        public DbSet<RefreshToken> RefreshTokensTab { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        }
    }
}
