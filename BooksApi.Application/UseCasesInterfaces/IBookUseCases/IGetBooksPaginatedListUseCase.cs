using BooksApi.Domain.Common;
using BooksApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IBookUseCases
{
    public interface IGetBooksPaginatedListUseCase
    {
        Task<PaginatedList<Book>> GetBooksPagList(int pageIndex, int pageSize, string? cat, int? authorId, CancellationToken cancellationToken);
    }
}
