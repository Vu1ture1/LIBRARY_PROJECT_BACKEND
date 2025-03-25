using BooksApi.DbService;
using BooksApi.Entities;
using BooksApi.Pagination;
using BooksApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Numerics;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;

namespace BooksApi.SqliteRepositoryServices
{
    public class BookRepositorySqlite : IBookRepository
    {
        private readonly AppDbContext Db;
        public BookRepositorySqlite(AppDbContext Db)
        {
            this.Db = Db;
        }
        public async Task<PaginatedList<Book>> GetBooks(int pageIndex, int pageSize, string? cat, int? authorId)
        {
            var books_table = Db.BooksTab.AsTracking();

            int count = 0;

            if (!string.IsNullOrEmpty(cat))
            {
                books_table = books_table.Where(b => b.Genre == cat);
                count = await books_table.CountAsync();
            }

            if (authorId.HasValue)
            {
                books_table = books_table.Where(b => b.AuthorId == authorId);
                count = await books_table.CountAsync();
            }

            var books = await books_table
                      .Include(b => b.book_author)
                      .Skip((pageIndex - 1) * pageSize)
                      .Take(pageSize)
                      .ToListAsync();

            int changes = 0;

            foreach (Book bk in books) {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now) 
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;
                    changes++;
                }
            }

            if (changes > 0) {
                await Db.SaveChangesAsync();
            }

            

            if (count == 0) { count = await Db.BooksTab.CountAsync(); }

            var totalPages = (int)Math.Ceiling(count / (double)pageSize); 

            return new PaginatedList<Book>(books, pageIndex, totalPages);
        }
        public async Task<Book> GetBookById(int bookId)
        {
            var book = await Db.BooksTab.Include(bk => bk.book_author).FirstOrDefaultAsync(bk => bk.Id == bookId);

            if (book != null && book.UserThatGetBook != 0 && book.ReturnBy < DateTime.Now)
            {
                book.BorrowedAt = DateTime.MinValue;
                book.ReturnBy = DateTime.MinValue;
                book.UserThatGetBook = 0;

                await Db.SaveChangesAsync();
            }

            return book;
        }
        public async Task<Book> GetBookByISBN(string ISBN)
        {
            var book = await Db.BooksTab.Include(bk => bk.book_author).FirstOrDefaultAsync(bk => bk.ISBN == ISBN);

            if (book != null && book.UserThatGetBook != 0 && book.ReturnBy < DateTime.Now)
            {
                book.BorrowedAt = DateTime.MinValue;
                book.ReturnBy = DateTime.MinValue;
                book.UserThatGetBook = 0;

                await Db.SaveChangesAsync();
            }

            return book;
        }
        public async Task AddBook(Book book)
        {
            await Db.BooksTab.AddAsync(book);

            await Db.SaveChangesAsync();
        }
        public async Task ChangeBook(int Id, Book changed_book)
        {
            var existingBook = await Db.BooksTab.FindAsync(Id);

            if (existingBook == null) { return; }

            existingBook.ISBN = changed_book.ISBN;
            existingBook.Name = changed_book.Name;
            existingBook.Genre = changed_book.Genre;
            existingBook.Description = changed_book.Description;
            existingBook.AuthorId = changed_book.AuthorId;
            existingBook.Image = changed_book.Image;

            await Db.SaveChangesAsync();
        }
        public async Task RemoveBook(int Id)
        {
            var existingBook = await Db.BooksTab.FindAsync(Id);

            if (existingBook == null) { return; }

            Db.BooksTab.Remove(existingBook);

            await Db.SaveChangesAsync();
        }
        public async Task GiveBookToUser(int bookId, int userId)
        {
            var book = await Db.BooksTab.FindAsync(bookId);

            if (book == null) { return; }

            var user = await Db.UsersTab.FindAsync(userId);

            if (user == null) { return; }   

            if (book != null) {
                book.BorrowedAt = DateTime.Now;
                book.ReturnBy = DateTime.Now.AddDays(14);
                book.UserThatGetBook = user.Id;
                await Db.SaveChangesAsync();
            }
        }
        public async Task<List<Book>> FindBookBySubstringName(string name) 
        {
            var name_lower = name.ToLower();

            int pageSize = 20;
            int pageNumber = 0;
            
            List<string> selected_Names = new List<string>();

            while (true)
            {
                var bookNames = await Db.BooksTab
                    .AsTracking()
                    .Select(b => b.Name)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize) 
                    .ToListAsync();

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

            var books = await Db.BooksTab
                .AsTracking()
                .Include(b => b.book_author)
                .Where(b => selected_Names.Contains(b.Name))
                .Take(10)
                .ToListAsync();

            int changes = 0;

            foreach (Book bk in books)
            {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now)
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;
                    changes++;
                }
            }

            if (changes > 0)
            {
                await Db.SaveChangesAsync();
            }

            return books;
        }
        public async Task<List<Book>> FindBooksByAuthor(int author_id) 
        {
            var books = await Db.BooksTab.AsTracking().Where(b => b.AuthorId == author_id).ToListAsync();

            int changes = 0;

            foreach (Book bk in books)
            {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now)
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;
                    changes++;
                }
            }

            if (changes > 0)
            {
                await Db.SaveChangesAsync();
            }

            return books;
        }
        public async Task<List<Book>> FindUserBooksOnHands(int user_id)
        {
            var books = await Db.BooksTab.AsTracking().Include(bk => bk.book_author).Where(b => b.UserThatGetBook == user_id).ToListAsync();

            List<Book> not_expired = new List<Book>();

            int changes = 0;

            foreach (Book bk in books)
            {
                if (bk.UserThatGetBook != 0 && bk.ReturnBy < DateTime.Now)
                {
                    bk.BorrowedAt = DateTime.MinValue;
                    bk.ReturnBy = DateTime.MinValue;
                    bk.UserThatGetBook = 0;
                    changes++;
                }
                else {
                    not_expired.Add(bk);
                }
            }

            if (changes > 0)
            {
                await Db.SaveChangesAsync();
            }

            return not_expired;
        }
    }
}
