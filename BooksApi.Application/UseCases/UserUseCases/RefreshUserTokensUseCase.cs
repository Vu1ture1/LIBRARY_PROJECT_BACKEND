using BooksApi.Application.Interfaces;
using BooksApi.Application.Responses;

namespace BooksApi.Application.UseCases.UserUseCases
{
    public class RefreshUserTokensUseCase
    {
        private ITokenGenerationService ts;

        private ITokenValidationService tvs;
        public RefreshUserTokensUseCase(ITokenGenerationService ts, ITokenValidationService tvs) 
        {
            this.ts = ts;
            this.tvs = tvs;
        }
        public async Task<ResponseForCookie> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var user = await tvs.ValidateRefreshToken(refreshToken, cancellationToken);

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
    }
}
