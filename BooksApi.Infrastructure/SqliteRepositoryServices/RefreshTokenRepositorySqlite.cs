using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using BooksApi.Infrastructure.DbService;
using BooksApi.Application.Services;
using BooksApi.Domain.Common;

namespace BooksApi.Infrastructure.SqliteRepositoryServices
{
    public class RefreshTokenRepositorySqlite : IRefreshTokenRepository
    {
        private readonly AppDbContext Db;
        public RefreshTokenRepositorySqlite(AppDbContext Db)
        {
            this.Db = Db;
        }
        public async Task AddRefreshToken(RefreshToken rt, CancellationToken cancellationToken)
        {
            await Db.RefreshTokensTab.AddAsync(rt, cancellationToken);

            await Db.SaveChangesAsync(cancellationToken);
        }
        public async Task<RefreshToken> GetRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            return await Db.RefreshTokensTab.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        }
        public async Task<List<RefreshToken>> GetAllUserRefreshTokens(int userId, CancellationToken cancellationToken)
        {
            return await Db.RefreshTokensTab.Where(rt => rt.UserId == userId).ToListAsync(cancellationToken);
        }
        public async Task DeleteUserRefreshTokens(List<RefreshToken> tokens, CancellationToken cancellationToken)
        {
            Db.RefreshTokensTab.RemoveRange(tokens);

            await Db.SaveChangesAsync(cancellationToken);
        }
    }
}
