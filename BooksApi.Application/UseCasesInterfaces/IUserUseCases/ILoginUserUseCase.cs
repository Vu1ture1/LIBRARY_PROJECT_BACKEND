using BooksApi.Application.DTOs;
using BooksApi.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IUserUseCases
{
    public interface ILoginUserUseCase
    {
        Task<ResponseForCookie> LoginUserAsync(UserLoginData loginDto, CancellationToken cancellationToken);
    }
}
