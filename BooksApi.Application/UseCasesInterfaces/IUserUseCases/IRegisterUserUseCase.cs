using BooksApi.Application.DTOs;
using BooksApi.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.UseCasesInterfaces.IUserUseCases
{
    public interface IRegisterUserUseCase
    {
        Task<ResponseForCookie> RegisterAsync(UserLoginData registerDto, CancellationToken cancellationToken);
    }
}
