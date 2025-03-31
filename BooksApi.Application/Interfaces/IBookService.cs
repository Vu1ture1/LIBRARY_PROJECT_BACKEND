using BooksApi.Application.DTOs;
using BooksApi.Domain.Common;
using BooksApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Interfaces
{
    public interface IBookService
    {
        Task<Book> GetBookById(int id, CancellationToken cancellationToken);
        Task<PaginatedList<Book>> GetBooksPagList(int pageIndex, int pageSize, string? cat, int? authorId, CancellationToken cancellationToken);
        Task<Book> GetBookByISBN(string ISBN, CancellationToken cancellationToken);
        Task GiveBookOnHands(int id, int userId, CancellationToken cancellationToken);
        Task<List<Book>> GetAllBooksOnHands(int userId, CancellationToken cancellationToken);
        Task AddBook(BookData bkDTO, CancellationToken cancellationToken);
        Task ChangeBook(int id, BookUpdateData bkDTO, CancellationToken cancellationToken);
        Task DeleteBook(int id, CancellationToken cancellationToken);
        Task<List<Book>> FindBooksByAuthor(int author_id, CancellationToken cancellationToken);
        Task<ActionResult<List<Book>>> FindBooksByName(string? book_subname, CancellationToken cancellationToken);
    }

}
