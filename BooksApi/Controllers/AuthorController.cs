using AutoMapper;
using BooksApi.Application.Exceptions.AuthorExceptions;
using BooksApi.Domain.Entities;
using BooksApi.Application.Services;
using BooksApi.Domain.Common;
using BooksApi.Application.DTOs;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BooksApi.Application.Responses;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCases.UserUseCases;
using BooksApi.Application.UseCases.AuthorUseCases;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : Controller
    {
        public AuthorController(){}

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetOneAuthorById([FromServices] GetOneAuthorByIdUseCase use_case, int id, CancellationToken cancellationToken)
        {
            var author = await use_case.GetOneAuthorById(id, cancellationToken);

            return Ok(author);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Author>>> GetAllAuthors([FromServices] GetAllAuthorsUseCase use_case, CancellationToken cancellationToken)
        {
            var authors = await use_case.GetAllAuthors(cancellationToken);

            return Ok(authors);
        }

        [HttpPost("add")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddAuthor([FromServices] AddAuthorUseCase use_case, [FromBody] AuthorData authorDTO, CancellationToken cancellationToken)
        {
            await use_case.AddAuthor(authorDTO, cancellationToken);

            return Ok();
        }

        [HttpPut("change/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeAuthor([FromServices] ChangeAuthorUseCase use_case, int id, [FromBody] AuthorData authorDTO, CancellationToken cancellationToken)
        {
            await use_case.ChangeAuthor(id, authorDTO, cancellationToken);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAuthor([FromServices] DeleteAuthorUseCase use_case, int id, CancellationToken cancellationToken)
        {
            await use_case.DeleteAuthor(id, cancellationToken);

            return Ok();
        }
    }
}
