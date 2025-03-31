using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Infrastructure.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;
using BooksApi.Domain.Entities;

namespace BooksApi.Infrastructure.DbService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
