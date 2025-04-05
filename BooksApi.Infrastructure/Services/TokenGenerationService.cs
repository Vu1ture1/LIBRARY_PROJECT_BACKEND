using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Threading;
using BooksApi.Application.Responses;
using BooksApi.Application.Interfaces;

namespace BooksApi.Infrastructure.Services
{
    public class TokenGenerationService : ITokenGenerationService
    {
        private IConfiguration Configuration { get; }

        private IRefreshTokenRepository rtr;
        public TokenGenerationService(IConfiguration config, IRefreshTokenRepository rtr)
        {
            Configuration = config;

            this.rtr = rtr;
        }
        public async Task<Response> GenerateJWTToken(User user, CancellationToken cancellationToken)
        {
            var claims = new List<Claim> {
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            };

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Configuration.GetValue<int>("ApplicationSettings:ExpirationInMinutes")),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_Secret"])
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = new RefreshToken();

            refreshToken.User = user;

            refreshToken.ExpiredDate = DateTime.UtcNow.AddDays(7);

            refreshToken.Token = GenerateRefreshToken();

            return new Response(accessToken, refreshToken);
        }
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
