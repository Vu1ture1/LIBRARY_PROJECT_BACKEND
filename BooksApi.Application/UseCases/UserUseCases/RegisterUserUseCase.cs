using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Application.Exceptions.UserExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.Responses;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;

namespace BooksApi.Application.UseCases.UserUseCases
{
    public class RegisterUserUseCase
    {
        private IUserRepository ur;

        private ITokenGenerationService ts;

        private IEncryptService es;

        private readonly IMapper mapper;

        public RegisterUserUseCase(IUserRepository ur, ITokenGenerationService ts, IEncryptService es, IMapper mapper)
        {
            this.ur = ur;
            this.ts = ts;
            this.es = es;
            this.mapper = mapper;
        }
        public async Task<ResponseForCookie> RegisterAsync(UserLoginData registerDto, CancellationToken cancellationToken)
        {
            var user = mapper.Map<User>(registerDto);

            user.Password = es.HashPassword(user.Password, user.Email);

            var user_check = await ur.GetUserByEmail(user.Email, cancellationToken);

            if (user_check != null)
            {
                throw new EmailAlreadyRegisteredException();
            }

            await ur.AddUser(user, cancellationToken);

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
    }
}
