using BooksApi.DbService;
using BooksApi.Entities;
using BooksApi.FileService;
using BooksApi.Pagination;
using BooksApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BooksApi.SqliteRepositoryServices
{
    public class AuthorRepositorySqlite : IAuthorRepository
    {
        private readonly AppDbContext Db;

        private IFileService fs;
        public AuthorRepositorySqlite(AppDbContext Db, IFileService fs)
        {
            this.Db = Db;
            this.fs = fs;
        }
        public async Task<List<Author>> GetAllAuthors() 
        {
            var authors = await Db.AuthorsTab
                .ToListAsync();

            return new List<Author>(authors);
        }
        public async Task<Author> GetAuthorById(int Id)
        {
            var author = await Db.AuthorsTab.FirstOrDefaultAsync(auth => auth.Id == Id);

            return author;
        }
        public async Task ChangeAuthor(int Id, Author changed_autor)
        {
            var existingAuthor = await Db.AuthorsTab.FindAsync(Id);

            if (existingAuthor == null) { return; }

            existingAuthor.Surname = changed_autor.Surname;
            existingAuthor.Counry = changed_autor.Counry;
            existingAuthor.Name = changed_autor.Name;
            existingAuthor.BornDate = changed_autor.BornDate;

            await Db.SaveChangesAsync();
        }
        public async Task RemoveAuthor(int Id)
        {
            var existingAuthor = await Db.AuthorsTab.FindAsync(Id);

            if (existingAuthor == null) { return; }

            var booksToRemove = await Db.BooksTab.Where(b => b.AuthorId == Id).ToListAsync();
            Db.BooksTab.RemoveRange(booksToRemove);

            Db.AuthorsTab.Remove(existingAuthor);

            foreach (Book bk in booksToRemove) {
                if (bk.Image != "Default.jpg") 
                {
                    fs.DeleteFile(bk.Image);
                }
            }

            await Db.SaveChangesAsync();
        }
        public async Task AddAuthor(Author autor)
        {
            await Db.AuthorsTab.AddAsync(autor);

            await Db.SaveChangesAsync();
        }
    }
}
