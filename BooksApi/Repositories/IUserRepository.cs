using BooksApi.Entities;

namespace BooksApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByEmailAndPassword(string email, string password);
        Task AddUser(User user);
    }
}
