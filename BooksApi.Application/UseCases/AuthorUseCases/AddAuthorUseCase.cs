using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Application.Interfaces;
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
    public class AddAuthorUseCase
    {
        private readonly IAuthorRepository ar;

        private readonly IMapper mapper;
        public AddAuthorUseCase(IAuthorRepository ar, IMapper mapper)
        {
            this.ar = ar;
            this.mapper = mapper;
        }
        public async Task AddAuthor(AuthorData auth_data, CancellationToken cancellationToken)
        {
            var auth = mapper.Map<Author>(auth_data);

            await ar.AddAuthor(auth, cancellationToken);

            return;
        }
    }
}
