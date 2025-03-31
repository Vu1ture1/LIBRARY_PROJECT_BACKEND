using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Application.Interfaces;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Exceptions.AuthorExceptions;
using BooksApi.Domain.Repositories;
using BooksApi.Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository ar;
        
        private readonly IBookRepository br;

        private IFileService fs;

        private readonly IMapper mapper;
        public AuthorService(IAuthorRepository ar, IBookRepository br, IFileService fs, IMapper mapper)
        {
            this.ar = ar;
            this.br = br;
            this.fs = fs;
            this.mapper = mapper;
        }

        public async Task<Author> GetOneAuthorById(int id, CancellationToken cancellationToken)
        {
            var author = await ar.GetAuthorById(id, cancellationToken);

            if (author == null)
            {
                throw new AuthorNotFoundException();
            }

            return author;
        }

        public async Task<ActionResult<List<Author>>> GetAllAuthors(CancellationToken cancellationToken)
        {
            var authors = await ar.GetAllAuthors(cancellationToken);

            if (authors == null || authors.Count == 0)
            {
                throw new AuthorNotFoundException();
            }

            return authors;
        }

        public async Task AddAuthor([FromBody] AuthorData auth_data, CancellationToken cancellationToken)
        {
            var auth = mapper.Map<Author>(auth_data);

            await ar.AddAuthor(auth, cancellationToken);

            return;
        }

        public async Task ChangeAuthor(int id, [FromBody] AuthorData auth_data, CancellationToken cancellationToken)
        {
            var existingAuthor = await ar.GetAuthorById(id, cancellationToken);

            if (existingAuthor == null)
            {
                throw new AuthorNotFoundException(id);
            }

            mapper.Map(auth_data, existingAuthor);

            await ar.ChangeAuthor(id, existingAuthor, cancellationToken);

            return;
        }

        public async Task DeleteAuthor(int id, CancellationToken cancellationToken)
        {
            var existingAuthor = await ar.GetAuthorById(id, cancellationToken);

            if (existingAuthor == null)
            {
                throw new AuthorNotFoundException(id);
            }

            List<Book> books_to_delete = await br.FindBooksByAuthor(id, cancellationToken);

            if (books_to_delete != null || books_to_delete.Count != 0) 
            {
                foreach (Book bk in books_to_delete) 
                {
                    await br.RemoveBook(bk.Id, cancellationToken);

                    if (bk.Image != "Default.jpg")
                    {
                        fs.DeleteFile(bk.Image, cancellationToken);
                    }
                }
            }

            await ar.RemoveAuthor(id, cancellationToken);

            return;
        }
    }
}
