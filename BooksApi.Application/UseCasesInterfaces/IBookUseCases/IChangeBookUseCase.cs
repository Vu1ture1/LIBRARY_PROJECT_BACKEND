using BooksApi.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IBookUseCases
{
    public interface IChangeBookUseCase
    {
        Task ChangeBook(int id, BookUpdateData bkDTO, CancellationToken cancellationToken);
    }
}
