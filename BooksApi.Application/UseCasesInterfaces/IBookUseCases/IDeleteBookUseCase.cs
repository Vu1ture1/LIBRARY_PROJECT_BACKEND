using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IBookUseCases
{
    public interface IDeleteBookUseCase
    {
        Task DeleteBook(int id, CancellationToken cancellationToken);
    }
}
