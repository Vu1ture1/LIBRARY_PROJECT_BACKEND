using AutoMapper;
using BooksApi.Application.DTOs;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Exceptions.UserExceptions;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BooksApi.Domain.Responses;
using BooksApi.Application.Interfaces;


namespace BooksApi.Application.Services
{
    public class UserService : IUserService
    {
        private IUserRepository ur;

        private IRefreshTokenRepository rtr;
        private ITokenService ts;

        private readonly IMapper mapper;
        public UserService(IUserRepository ur, IRefreshTokenRepository rtr, IMapper mapper, ITokenService ts)
        {
            this.ur = ur;
            this.rtr = rtr;
            this.ts = ts;
            this.mapper = mapper;
        }
        public async Task<ResponseForCookie> LoginUserAsync(UserLoginData loginDto, CancellationToken cancellationToken)
        {
            string encryptedPassword = HashPassword(loginDto.Password, loginDto.Email);

            var user = await ur.GetUserByEmailAndPassword(loginDto.Email, encryptedPassword, cancellationToken);

            if (user == null)
            {
                throw new InvalidCredentialsException();
            }

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
        public async Task<ResponseForCookie> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken)
        {
            if (refreshToken == null)
            {
                throw new ExpiredRefreshTokenException();
            }

            var storedToken = await rtr.GetRefreshToken(refreshToken, cancellationToken);

            if (storedToken == null || storedToken.ExpiredDate < DateTime.UtcNow)
            {
                throw new ExpiredRefreshTokenException();
            }

            var response = await ts.GenerateJWTToken(storedToken.User, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
        public async Task<ResponseForCookie> RegisterAsync(UserLoginData registerDto, CancellationToken cancellationToken)
        {
            var user = mapper.Map<User>(registerDto);

            user.Password = HashPassword(user.Password, user.Email);

            var user_check = await ur.GetUserByEmail(user.Email, cancellationToken);

            if (user_check != null)
            {
                throw new EmailAlreadyRegisteredException();
            }

            await ur.AddUser(user, cancellationToken);

            var response = await ts.GenerateJWTToken(user, cancellationToken);

            return new ResponseForCookie(response.accessToken, response.refreshToken.Token, response.refreshToken.ExpiredDate);
        }
        public static string HashPassword(string password, string email)
        {
            byte[] salt = Encoding.UTF8.GetBytes(email);

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }
        public static bool VerifyPassword(string hashedPassword, string enteredPassword, string email)
        {
            byte[] salt = Encoding.UTF8.GetBytes(email);

            string enteredHashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return enteredHashedPassword == hashedPassword;
        }
    }
}
