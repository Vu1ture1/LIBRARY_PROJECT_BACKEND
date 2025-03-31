using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using BooksApi.Infrastructure.DbService;
using System.Threading;
using BooksApi.Application.Interfaces;

namespace BooksApi.Infrastructure.SqliteRepositoryServices
{
    public class AuthorRepositorySqlite : IAuthorRepository
    {
        private readonly AppDbContext Db;
        public AuthorRepositorySqlite(AppDbContext Db, IFileService fs)
        {
            this.Db = Db;
        }
        public async Task<List<Author>> GetAllAuthors(CancellationToken cancellationToken)
        {
            return await Db.AuthorsTab.ToListAsync(cancellationToken);
        }
        public async Task<Author> GetAuthorById(int Id, CancellationToken cancellationToken)
        {
            return await Db.AuthorsTab.FirstOrDefaultAsync(auth => auth.Id == Id, cancellationToken);
        }
        public async Task ChangeAuthor(int Id, Author changed_autor, CancellationToken cancellationToken)
        {
            var existingAuthor = await Db.AuthorsTab.FindAsync(Id, cancellationToken);

            Db.AuthorsTab.Entry(existingAuthor).CurrentValues.SetValues(changed_autor);

            await Db.SaveChangesAsync(cancellationToken);
        }
        public async Task RemoveAuthor(int Id, CancellationToken cancellationToken)
        {
            var existingAuthor = await Db.AuthorsTab.FindAsync(Id, cancellationToken);

            Db.AuthorsTab.Remove(existingAuthor);

            await Db.SaveChangesAsync(cancellationToken);
        }
        public async Task AddAuthor(Author autor, CancellationToken cancellationToken)
        {
            await Db.AuthorsTab.AddAsync(autor, cancellationToken);

            await Db.SaveChangesAsync(cancellationToken);
        }
    }
}
