using BooksApi.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IUserUseCases
{
    public interface IRefreshUserTokensUseCase
    {
        Task<ResponseForCookie> RefreshTokenAsync(string refreshTokenStr, CancellationToken cancellationToken);
    }
}
