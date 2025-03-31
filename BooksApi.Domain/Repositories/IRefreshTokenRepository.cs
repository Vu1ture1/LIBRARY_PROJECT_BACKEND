using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;

namespace BooksApi.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshToken(RefreshToken rt, CancellationToken cancellationToken);
        Task<RefreshToken> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task<List<RefreshToken>> GetAllUserRefreshTokens(int userId, CancellationToken cancellationToken);
        Task DeleteUserRefreshTokens(List<RefreshToken> tokens, CancellationToken cancellationToken);
    }
}