using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using BooksApi.Infrastructure.DbService;
using BooksApi.Application.Services;
using BooksApi.Domain.Common;

namespace BooksApi.Infrastructure.SqliteRepositoryServices
{
    public class UserRepositorySqlite : IUserRepository
    {
        private readonly AppDbContext Db;
        public UserRepositorySqlite(AppDbContext Db)
        {
            this.Db = Db;
        }
        public async Task<User> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            return await Db.UsersTab.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
        public async Task<User> GetUserByEmailAndPassword(string email, string password, CancellationToken cancellationToken)
        {
            return await Db.UsersTab.FirstOrDefaultAsync(u => u.Email == email && u.Password == password, cancellationToken);
        }
        public async Task AddUser(User user, CancellationToken cancellationToken)
        {
            await Db.UsersTab.AddAsync(user, cancellationToken);

            await Db.SaveChangesAsync(cancellationToken);
        }
    }
}
