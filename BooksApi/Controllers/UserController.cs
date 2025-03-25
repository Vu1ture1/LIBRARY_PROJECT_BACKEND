using BooksApi.DbService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BooksApi.Helpers;
using BooksApi.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BooksApi.PostEntities;
using BooksApi.Repositories;
using BooksApi.CustomExceptions;
using AutoMapper;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private IUserRepository ur;

        private IRefreshTokenRepository rtr;

        private readonly IMapper mapper;
        private TokenService ts { get; }
 
        public UserController(IUserRepository ur, IRefreshTokenRepository rtr, IMapper mapper, TokenService ts)
        {
            this.ur = ur;
            this.rtr = rtr;
            this.ts = ts;
            this.mapper = mapper;
        }

        [HttpPost("login/refresh")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var storedToken = await rtr.GetRefreshToken(refreshToken);

            if (storedToken == null)
            {
                //return Unauthorized("Refresh token недействителен.");
                throw new InvalidRefreshTokenException();
            }

            if (storedToken.ExpiredDate < DateTime.UtcNow)
            {
                //return Unauthorized("Refresh token истёк. Пожалуйста, войдите снова.");
                throw new ExpiredRefreshTokenException();
            }

            var response = await ts.GenerateJWTToken(storedToken.User); // public record Response(string accessToken, string refreshToken);

            Response.Cookies.Append("refreshToken", response.refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = (response.refreshToken.ExpiredDate),
                Path = "/"
            });

            return Ok(new { accessToken = response.accessToken });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] UserLoginData loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string email = loginDto.Email;
            string password = loginDto.Password;

            string encrypted_passw = HashPassword(password, email);

            var user = await ur.GetUserByEmailAndPassword(email, encrypted_passw);

            if (user == null)
            {
                //return Unauthorized("Неверный email или пароль.");
                throw new InvalidCredentialsException();
            }

            //var res = await ts.GenerateJWTToken(user);

            //return Ok(res);

            var response = await ts.GenerateJWTToken(user); // public record Response(string accessToken, string refreshToken);

            Response.Cookies.Append("refreshToken", response.refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = (response.refreshToken.ExpiredDate),
                Path = "/",
            });

            return Ok(new { accessToken = response.accessToken });
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterUser([FromBody] UserLoginData user_data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string email = user_data.Email;
            string password = user_data.Password;

            string encrypted_passw = HashPassword(password, email);

            //var user_check = await Db.UsersTab.FirstOrDefaultAsync(u => u.Email == email);

            var user_check = await ur.GetUserByEmail(email);

            if (user_check != null)
            {
                //return Unauthorized("Такой email уже зарегестрирован.");
                throw new EmailAlreadyRegisteredException();
            }

            var user = mapper.Map<User>(user_data);

            user.Password = encrypted_passw;

            await ur.AddUser(user);

            var response = await ts.GenerateJWTToken(user); // public record Response(string accessToken, string refreshToken);

            Response.Cookies.Append("refreshToken", response.refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = (response.refreshToken.ExpiredDate),
                Path = "/",
            });

            return Ok(new { accessToken = response.accessToken });

            //return Ok(await ts.GenerateJWTToken(user));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<string>> LogoutUser()
        {
            if (Request.Cookies["refreshToken"] != null)
            {
                Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, 
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(-1), 
                    Path = "/"
                });

                return Ok(new { message = "Вы успешно вышли из системы." });
            }

            return Ok(new { message = "Пользователь уже вышел, выход не требуется." });
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