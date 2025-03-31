using BooksApi.Application.DTOs;
using BooksApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Interfaces
{
    public interface IAuthorService
    {
        Task<Author> GetOneAuthorById(int id, CancellationToken cancellationToken);

        Task<ActionResult<List<Author>>> GetAllAuthors(CancellationToken cancellationToken);

        Task AddAuthor(AuthorData auth_data, CancellationToken cancellationToken);

        Task ChangeAuthor(int id, AuthorData auth_data, CancellationToken cancellationToken);

        Task DeleteAuthor(int id, CancellationToken cancellationToken);
    }
}
