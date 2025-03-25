using BooksApi.DbService;
using BooksApi.Entities;
using BooksApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.SqliteRepositoryServices
{
    public class UserRepositorySqlite : IUserRepository
    {
        private readonly AppDbContext Db;
        public UserRepositorySqlite(AppDbContext Db) 
        {
            this.Db = Db;
        }
        public async Task<User> GetUserByEmail(string email) { 
            return await Db.UsersTab.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByEmailAndPassword(string email, string password) { 
            return await Db.UsersTab.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
        public async Task AddUser(User user) {
            await Db.UsersTab.AddAsync(user);

            await Db.SaveChangesAsync();
        }
    }
}
