using BooksApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IAuthorUseCases
{
    public interface IGetOneAuthorByIdUseCase
    {
        Task<Author> GetOneAuthorById(int id, CancellationToken cancellationToken);
    }
}
