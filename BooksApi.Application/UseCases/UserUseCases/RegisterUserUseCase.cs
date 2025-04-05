using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Application.Exceptions.UserExceptions;
using BooksApi.Application.Interfaces;
using BooksApi.Application.Responses;
using BooksApi.Application.UseCasesInterfaces.IUserUseCases;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;

namespace BooksApi.Application.UseCases.UserUseCases
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private IUserRepository ur;

        private ITokenGenerationService ts;

        private IEncryptService es;

        private readonly IMapper mapper;

        private IRefreshTokenRepository rtr;
        public RegisterUserUseCase(IUserRepository ur, IRefreshTokenRepository rtr, ITokenGenerationService ts, IEncryptService es, IMapper mapper)
        {
            this.ur = ur;
            this.rtr = rtr;
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


            var oldTokens = await rtr.GetAllUserRefreshTokens(user.Id, cancellationToken);

            if (oldTokens != null)
                await rtr.DeleteUserRefreshTokens(oldTokens, cancellationToken);

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            await rtr.AddRefreshToken(response.refreshToken, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
    }
}
