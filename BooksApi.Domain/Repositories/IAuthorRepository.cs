using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;

namespace BooksApi.Domain.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAuthors(CancellationToken cancellationToken);
        Task<Author> GetAuthorById(int Id, CancellationToken cancellationToken);
        Task ChangeAuthor(int Id, Author changed_autor, CancellationToken cancellationToken);
        Task RemoveAuthor(int Id, CancellationToken cancellationToken);
        Task AddAuthor(Author autor, CancellationToken cancellationToken);
    }
}