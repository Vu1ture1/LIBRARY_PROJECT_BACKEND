using BooksApi.Application.Interfaces;
using BooksApi.Application.Responses;
using BooksApi.Application.UseCasesInterfaces.IUserUseCases;
using BooksApi.Domain.Repositories;

namespace BooksApi.Application.UseCases.UserUseCases
{
    public class RefreshUserTokensUseCase : IRefreshUserTokensUseCase
    {
        private ITokenGenerationService ts;

        private ITokenValidationService tvs;

        private IRefreshTokenRepository rtr;
        public RefreshUserTokensUseCase(ITokenGenerationService ts, ITokenValidationService tvs, IRefreshTokenRepository rtr) 
        {
            this.ts = ts;
            this.tvs = tvs;
            this.rtr = rtr;
        }
        public async Task<ResponseForCookie> RefreshTokenAsync(string refreshTokenStr, CancellationToken cancellationToken)
        {
            var refreshToken = await rtr.GetRefreshToken(refreshTokenStr, cancellationToken);

            var user = tvs.ValidateRefreshToken(refreshToken, cancellationToken);

            var oldTokens = await rtr.GetAllUserRefreshTokens(user.Id, cancellationToken);

            if (oldTokens != null)
                await rtr.DeleteUserRefreshTokens(oldTokens, cancellationToken);

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            await rtr.AddRefreshToken(response.refreshToken, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
    }
}
