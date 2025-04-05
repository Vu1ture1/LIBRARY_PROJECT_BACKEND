using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BooksApi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Application.Exceptions.UserExceptions;
using System.Threading;
using BooksApi.Domain.Repositories;
using BooksApi.Domain.Entities;

namespace BooksApi.Application.Services
{
    public class TokenValidationService : ITokenValidationService
    {
        public TokenValidationService(){}
        public User ValidateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            if (refreshToken == null || refreshToken.ExpiredDate < DateTime.UtcNow)
            {
                throw new ExpiredRefreshTokenException();
            }

            return refreshToken.User;
        }
    }

}
