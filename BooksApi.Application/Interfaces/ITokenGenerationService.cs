using BooksApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Application.Responses;

namespace BooksApi.Application.Interfaces
{
    public interface ITokenGenerationService
    {
        Task<Response> GenerateJWTToken(User user, CancellationToken cancellationToken);
    }

}
