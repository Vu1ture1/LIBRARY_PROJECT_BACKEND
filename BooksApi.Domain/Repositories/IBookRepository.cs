using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Common;

namespace BooksApi.Domain.Repositories
{
    public interface IBookRepository
    {
        Task<PaginatedList<Book>> GetBooks(int pageIndex, int pageSize, CancellationToken cancellationToken);

        Task<PaginatedList<Book>> GetBooksByCat(int pageIndex, int pageSize, string cat, CancellationToken cancellationToken);

        Task<PaginatedList<Book>> GetBooksByAuthor(int pageIndex, int pageSize, int authorId, CancellationToken cancellationToken);

        Task<PaginatedList<Book>> GetBooksByAll(int pageIndex, int pageSize, string cat, int authorId, CancellationToken cancellationToken);

        Task<Book> GetBookById(int bookId, CancellationToken cancellationToken);
        Task<Book> GetBookByISBN(string ISBN, CancellationToken cancellationToken);
        Task AddBook(Book book, CancellationToken cancellationToken);
        Task ChangeBook(Book changed_book, CancellationToken cancellationToken);
        Task RemoveBook(Book book, CancellationToken cancellationToken);
        Task<List<Book>> FindBookBySubstringName(string name, CancellationToken cancellationToken);
        Task<List<Book>> FindBooksByAuthor(int author_id, CancellationToken cancellationToken);

        Task<List<Book>> FindUserBooksOnHands(int user_id, CancellationToken cancellationToken);
    }
}