using Microsoft.AspNetCore.Mvc;
using BooksApi.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using BooksApi.Application.DTOs;
using BooksApi.Domain.Repositories;
using BooksApi.Domain.Exceptions.UserExceptions;
using AutoMapper;
using System.Net;
using System;
using Newtonsoft.Json;
using BooksApi.Application.Interfaces;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private IUserService us;
        public UserController(IUserService us)
        {
            this.us = us;
        }

        [HttpPost("login/refresh")]
        public async Task<ActionResult<string>> RefreshToken(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var cookieRes = await us.RefreshTokenAsync(refreshToken, cancellationToken);

            Response.Cookies.Append("refreshToken", cookieRes.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = (cookieRes.ExpiredDate),
                Path = "/"
            });

            return Ok(new { accessToken = cookieRes.AccessToken });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] UserLoginData loginDto, CancellationToken cancellationToken)
        {
            var cookieRes = await us.LoginUserAsync(loginDto, cancellationToken);

            Response.Cookies.Append("refreshToken", cookieRes.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = (cookieRes.ExpiredDate),
                Path = "/"
            });

            return Ok(new { accessToken = cookieRes.AccessToken });
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterUser([FromBody] UserLoginData regDto, CancellationToken cancellationToken)
        {
            var cookieRes = await us.RegisterAsync(regDto, cancellationToken);

            Response.Cookies.Append("refreshToken", cookieRes.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = (cookieRes.ExpiredDate),
                Path = "/"
            });

            return Ok(new { accessToken = cookieRes.AccessToken });
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

            return NoContent();
        }   
    }
}