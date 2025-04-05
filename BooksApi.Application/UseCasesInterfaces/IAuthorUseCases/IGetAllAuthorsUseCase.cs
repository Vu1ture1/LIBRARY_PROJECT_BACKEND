using BooksApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IAuthorUseCases
{
    public interface IGetAllAuthorsUseCase
    {
        Task<ActionResult<List<Author>>> GetAllAuthors(CancellationToken cancellationToken);
    }
}
