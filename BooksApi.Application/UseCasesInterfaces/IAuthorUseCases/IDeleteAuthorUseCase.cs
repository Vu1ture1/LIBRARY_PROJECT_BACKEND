using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IAuthorUseCases
{
    public interface IDeleteAuthorUseCase
    {
        Task DeleteAuthor(int id, CancellationToken cancellationToken);
    }
}
