using BooksApi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Interfaces
{
    public interface ITokenValidationService
    {
        public User ValidateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
    }
}
