using AutoMapper;
using BooksApi.Application.Exceptions.AuthorExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCasesInterfaces.IAuthorUseCases;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.AuthorUseCases
{
    public class GetAllAuthorsUseCase : IGetAllAuthorsUseCase
    {
        private readonly IAuthorRepository ar;
        public GetAllAuthorsUseCase(IAuthorRepository ar)
        {
            this.ar = ar;
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
    }
}
