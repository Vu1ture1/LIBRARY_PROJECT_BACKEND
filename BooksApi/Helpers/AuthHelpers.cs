using BooksApi.DbService;
using BooksApi.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BooksApi.Repositories;

namespace BooksApi.Helpers
{

    public class TokenService 
    {
        private IConfiguration Configuration { get; }

        private IRefreshTokenRepository rtr;

        public record Response(string accessToken, RefreshToken refreshToken);
        public TokenService(IConfiguration config, IRefreshTokenRepository rtr) {
           
            Configuration = config;

            this.rtr = rtr;
        }
        public async Task<Response> GenerateJWTToken(User user)
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

            var oldTokens = await rtr.GetAllUserRefreshTokens(user.Id);

            if(oldTokens != null)
                await rtr.DeleteUserRefreshTokens(oldTokens);

            await rtr.AddRefreshToken(refreshToken);

            return new Response(accessToken, refreshToken);
        }
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
