using BooksApi.Entities;

namespace BooksApi.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshToken(RefreshToken rt);
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        Task<List<RefreshToken>> GetAllUserRefreshTokens(int userId);
        Task DeleteUserRefreshTokens(List<RefreshToken> tokens);

        Task RefreshTokens(int userId, RefreshToken refreshToken);
    }
}
