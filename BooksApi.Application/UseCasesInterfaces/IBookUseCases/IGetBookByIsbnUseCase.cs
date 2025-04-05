using BooksApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IBookUseCases
{
    public interface IGetBookByIsbnUseCase
    {
        Task<Book> GetBookByISBN(string ISBN, CancellationToken cancellationToken);
    }
}
