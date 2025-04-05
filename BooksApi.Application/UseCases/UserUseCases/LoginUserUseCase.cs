using BooksApi.Application.DTOs;
using BooksApi.Application.Exceptions.UserExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.Responses;
using BooksApi.Application.UseCasesInterfaces.IUserUseCases;
using BooksApi.Domain.Repositories;

namespace BooksApi.Application.UseCases.UserUseCases
{
    public class LoginUserUseCase : ILoginUserUseCase
    {
        private IUserRepository ur;

        private ITokenGenerationService ts;

        private IRefreshTokenRepository rtr;

        private IEncryptService es;
        public LoginUserUseCase(IUserRepository ur, IRefreshTokenRepository rtr, ITokenGenerationService ts, IEncryptService es)
        {
            this.ur = ur;
            this.rtr = rtr;
            this.ts = ts;
            this.es = es;
        }

        public async Task<ResponseForCookie> LoginUserAsync(UserLoginData loginDto, CancellationToken cancellationToken)
        {
            string encryptedPassword = es.HashPassword(loginDto.Password, loginDto.Email);

            var user = await ur.GetUserByEmailAndPassword(loginDto.Email, encryptedPassword, cancellationToken);

            if (user == null)
            {
                throw new InvalidCredentialsException();
            }

            var oldTokens = await rtr.GetAllUserRefreshTokens(user.Id, cancellationToken);

            if (oldTokens != null)
                await rtr.DeleteUserRefreshTokens(oldTokens, cancellationToken);

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            await rtr.AddRefreshToken(response.refreshToken, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
    }
}
