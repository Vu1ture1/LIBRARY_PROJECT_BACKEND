using BooksApi.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IAuthorUseCases
{
    public interface IAddAuthorUseCase
    {
        Task AddAuthor(AuthorData auth_data, CancellationToken cancellationToken);
    }
}
