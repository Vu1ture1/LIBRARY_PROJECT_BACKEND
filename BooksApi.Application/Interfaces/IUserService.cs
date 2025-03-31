using BooksApi.Application.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Responses;

namespace BooksApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResponseForCookie> LoginUserAsync(UserLoginData loginDto, CancellationToken cancellationToken);
        Task<ResponseForCookie> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken);
        Task<ResponseForCookie> RegisterAsync(UserLoginData registerDto, CancellationToken cancellationToken);
    }

}
