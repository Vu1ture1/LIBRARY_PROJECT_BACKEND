using BooksApi.Application.DTOs;
using BooksApi.Application.Exceptions.UserExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.Responses;
using BooksApi.Domain.Repositories;

namespace BooksApi.Application.UseCases.UserUseCases
{
    public class LoginUserUseCase
    {
        private IUserRepository ur;

        private ITokenGenerationService ts;

        private IEncryptService es;
        public LoginUserUseCase(IUserRepository ur, ITokenGenerationService ts, IEncryptService es)
        {
            this.ur = ur;
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

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
    }
}
