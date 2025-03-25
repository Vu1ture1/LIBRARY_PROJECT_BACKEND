using BooksApi.Entities;
using BooksApi.Pagination;

namespace BooksApi.Repositories
{
    public interface IBookRepository
    {
        Task<PaginatedList<Book>> GetBooks(int pageIndex, int pageSize, string? cat, int? authorId);
        Task<Book> GetBookById(int bookId);
        Task<Book> GetBookByISBN(string ISBN);
        Task AddBook(Book book);
        Task ChangeBook(int Id, Book changed_book);
        Task RemoveBook(int Id);
        Task GiveBookToUser(int bookId, int userId);
        Task<List<Book>> FindBookBySubstringName(string name);
        Task<List<Book>> FindBooksByAuthor(int author_id);

        Task<List<Book>> FindUserBooksOnHands(int user_id);
    }
}
