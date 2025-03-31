using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;

namespace BooksApi.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email, CancellationToken cancellationToken);
        Task<User> GetUserByEmailAndPassword(string email, string password, CancellationToken cancellationToken);
        Task AddUser(User user, CancellationToken cancellationToken);
    }
}