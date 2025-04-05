using BooksApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IBookUseCases
{
    public interface IFindBooksByNameUseCase
    {
        Task<ActionResult<List<Book>>> FindBooksByName(string? book_subname, CancellationToken cancellationToken);
    }
}
