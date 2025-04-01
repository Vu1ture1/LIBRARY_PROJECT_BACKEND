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
    public class BookRepositorySqlite : IBookRepository 
    { 
        private readonly AppDbContext Db;
        public BookRepositorySqlite(AppDbContext Db)
        {
            this.Db = Db;
        }
        public async Task<PaginatedList<Book>> GetBooks(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var books_table = Db.BooksTab.AsTracking();

            var books = await books_table
                      .Include(b => b.book_author)
                      .Skip((pageIndex - 1) * pageSize)
                      .Take(pageSize)
                      .ToListAsync(cancellationToken);

            int count = await Db.BooksTab.CountAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PaginatedList<Book>(books, pageIndex, totalPages);
        }
        public async Task<PaginatedList<Book>> GetBooksByCat(int pageIndex, int pageSize, string cat, CancellationToken cancellationToken)
        {
            var books_table = Db.BooksTab.AsTracking();

            books_table = books_table.Where(b => b.Genre == cat);
            
            int count = await books_table.CountAsync(cancellationToken);

            var books = await books_table
                      .Include(b => b.book_author)
                      .Skip((pageIndex - 1) * pageSize)
                      .Take(pageSize)
                      .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PaginatedList<Book>(books, pageIndex, totalPages);
        }
        public async Task<PaginatedList<Book>> GetBooksByAuthor(int pageIndex, int pageSize, int authorId, CancellationToken cancellationToken)
        {
            var books_table = Db.BooksTab.AsTracking();

            books_table = books_table.Where(b => b.AuthorId == authorId);
            int count = await books_table.CountAsync(cancellationToken);

            var books = await books_table
                      .Include(b => b.book_author)
                      .Skip((pageIndex - 1) * pageSize)
                      .Take(pageSize)
                      .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PaginatedList<Book>(books, pageIndex, totalPages);
        }
        public async Task<PaginatedList<Book>> GetBooksByAll(int pageIndex, int pageSize, string cat, int authorId, CancellationToken cancellationToken)
        {
            var books_table = Db.BooksTab.AsTracking();

            books_table = books_table.Where(b => b.Genre == cat && b.AuthorId == authorId);
            
            int count = await books_table.CountAsync(cancellationToken);

            var books = await books_table
                      .Include(b => b.book_author)
                      .Skip((pageIndex - 1) * pageSize)
                      .Take(pageSize)
                      .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PaginatedList<Book>(books, pageIndex, totalPages);
        }
        public async Task<Book> GetBookById(int bookId, CancellationToken cancellationToken)
        {
            return await Db.BooksTab.Include(bk => bk.book_author).FirstOrDefaultAsync(bk => bk.Id == bookId, cancellationToken);
        }
        public async Task<Book> GetBookByISBN(string ISBN, CancellationToken cancellationToken)
        {
            return await Db.BooksTab.Include(bk => bk.book_author).FirstOrDefaultAsync(bk => bk.ISBN == ISBN, cancellationToken);
        }
        public async Task AddBook(Book book, CancellationToken cancellationToken)
        {
            await Db.BooksTab.AddAsync(book, cancellationToken);

            await Db.SaveChangesAsync(cancellationToken);
        }
        public async Task ChangeBook(int Id, Book changed_book, CancellationToken cancellationToken)
        {
            var existingBook = await Db.BooksTab.FindAsync(Id, cancellationToken);

            Db.BooksTab.Entry(existingBook).CurrentValues.SetValues(changed_book);

            await Db.SaveChangesAsync(cancellationToken);
        }
        public async Task RemoveBook(int Id, CancellationToken cancellationToken)
        {
            var existingBook = await Db.BooksTab.FindAsync(Id, cancellationToken);

            Db.BooksTab.Remove(existingBook);

            await Db.SaveChangesAsync(cancellationToken);
        }
        public async Task GiveBookToUser(int bookId, int userId, CancellationToken cancellationToken)
        {
            var book = await Db.BooksTab.FindAsync(bookId, cancellationToken);

            var user = await Db.UsersTab.FindAsync(userId, cancellationToken);

            book.BorrowedAt = DateTime.Now;
            book.ReturnBy = DateTime.Now.AddDays(14);
            book.UserThatGetBook = user.Id;
            
            await Db.SaveChangesAsync(cancellationToken);
        }
        
        // Из за того что SQlite не поддерживает кирилицу в операторе like, я порционно выгружаю книги и сохраняю те что подходят по условию
        public async Task<List<Book>> FindBookBySubstringName(string name, CancellationToken cancellationToken)
        {
            var name_lower = name.ToLower();

            int pageSize = 50;
            int pageNumber = 0;

            List<string> selected_Names = new List<string>();

            while (true)
            {
                var bookNames = await Db.BooksTab
                    .AsTracking()
                    .Select(b => b.Name)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                if (!bookNames.Any())
                    break;

                foreach (var name_ in bookNames)
                {
                    if (name_.ToLower().Contains(name_lower))
                    {
                        selected_Names.Add(name_);
                    }
                }

                bookNames.Clear();

                pageNumber++;
            }

            return await Db.BooksTab
                .AsTracking()
                .Include(b => b.book_author)
                .Where(b => selected_Names.Contains(b.Name))
                .Take(10)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Book>> FindBooksByAuthor(int author_id, CancellationToken cancellationToken)
        {
            return await Db.BooksTab.AsTracking().Where(b => b.AuthorId == author_id).ToListAsync(cancellationToken);
        }
        public async Task<List<Book>> FindUserBooksOnHands(int user_id, CancellationToken cancellationToken)
        {
            return await Db.BooksTab.AsTracking().Include(bk => bk.book_author).Where(b => b.UserThatGetBook == user_id).ToListAsync(cancellationToken);
        }
    }
}
