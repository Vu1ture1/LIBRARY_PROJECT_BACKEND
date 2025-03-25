using BooksApi.Entities;
using BooksApi.Pagination;

namespace BooksApi.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAuthors();
        Task<Author> GetAuthorById(int Id);
        Task ChangeAuthor(int Id, Author changed_autor);
        Task RemoveAuthor(int Id);
        Task AddAuthor(Author autor);
    }
}
