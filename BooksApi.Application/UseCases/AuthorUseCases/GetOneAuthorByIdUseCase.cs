using AutoMapper;
using BooksApi.Application.Exceptions.AuthorExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.AuthorUseCases
{
    public class GetOneAuthorByIdUseCase
    {
        private readonly IAuthorRepository ar;
        public GetOneAuthorByIdUseCase(IAuthorRepository ar)
        {
            this.ar = ar;
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
    }
}
