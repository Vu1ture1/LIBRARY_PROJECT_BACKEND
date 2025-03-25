using BooksApi.DbService;
using BooksApi.Entities;
using BooksApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace BooksApi.SqliteRepositoryServices
{
    public class RefreshTokenRepositorySqlite : IRefreshTokenRepository
    {
        private readonly AppDbContext Db;
        public RefreshTokenRepositorySqlite(AppDbContext Db)
        {
            this.Db = Db;
        }
        public async Task AddRefreshToken(RefreshToken rt) {
            await Db.RefreshTokensTab.AddAsync(rt);

            await Db.SaveChangesAsync();
        }
        public async Task<RefreshToken> GetRefreshToken(string refreshToken) {
            return await Db.RefreshTokensTab.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        }
        public async Task<List<RefreshToken>> GetAllUserRefreshTokens(int userId) {
            return await Db.RefreshTokensTab.Where(rt => rt.UserId == userId).ToListAsync();
        }
        public async Task DeleteUserRefreshTokens(List<RefreshToken> tokens) {
            Db.RefreshTokensTab.RemoveRange(tokens);

            await Db.SaveChangesAsync();
        }

        public async Task RefreshTokens(int userId, RefreshToken refreshToken)
        {
            using (var transaction = await Db.Database.BeginTransactionAsync())
            {
                try
                {
                    var oldToken = await Db.RefreshTokensTab.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken.Token);

                    if (oldToken != null)
                    {
                        Db.RefreshTokensTab.RemoveRange(oldToken);
                    }

                    await Db.RefreshTokensTab.AddAsync(oldToken);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Ошибка при обновлении токенов: {ex.Message}");
                    throw;
                }
            }
        }

    }
}
