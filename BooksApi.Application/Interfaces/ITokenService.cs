using BooksApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Responses;

namespace BooksApi.Application.Interfaces
{
    public interface ITokenService
    {
        Task<Response> GenerateJWTToken(User user, CancellationToken cancellationToken);
    }

}
