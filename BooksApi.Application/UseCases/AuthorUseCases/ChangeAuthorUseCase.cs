using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Application.Exceptions.AuthorExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCasesInterfaces.IAuthorUseCases;
using BooksApi.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCases.AuthorUseCases
{
    public class ChangeAuthorUseCase : IChangeAuthorUseCase
    {
        private readonly IAuthorRepository ar;

        private readonly IMapper mapper;
        public ChangeAuthorUseCase(IAuthorRepository ar, IMapper mapper)
        {
            this.ar = ar;
            this.mapper = mapper;
        }
        public async Task ChangeAuthor(int id, AuthorData auth_data, CancellationToken cancellationToken)
        {
            var existingAuthor = await ar.GetAuthorById(id, cancellationToken);

            if (existingAuthor == null)
            {
                throw new AuthorNotFoundException(id);
            }

            mapper.Map(auth_data, existingAuthor);

            await ar.ChangeAuthor(existingAuthor, cancellationToken);

            return;
        }
    }
}
