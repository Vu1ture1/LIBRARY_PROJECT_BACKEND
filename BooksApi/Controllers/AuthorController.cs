using AutoMapper;
using BooksApi.Domain.Exceptions.AuthorExceptions;
using BooksApi.Domain.Entities;
using BooksApi.Application.Services;
using BooksApi.Domain.Common;
using BooksApi.Application.DTOs;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BooksApi.Domain.Responses;
using BooksApi.Application.Interfaces;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : Controller
    {
        private IAuthorService _as;
        public AuthorController(IAuthorService _as)
        {
            this._as = _as;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetOneAuthorById(int id, CancellationToken cancellationToken)
        {
            var author = await _as.GetOneAuthorById(id, cancellationToken);

            return Ok(author);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Author>>> GetAllAuthors(CancellationToken cancellationToken)
        {
            var authors = await _as.GetAllAuthors(cancellationToken);

            return Ok(authors);
        }

        [HttpPost("add")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorData authorDTO, CancellationToken cancellationToken)
        {
            await _as.AddAuthor(authorDTO, cancellationToken);

            return Ok();
        }

        [HttpPut("change/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeAuthor(int id, [FromBody] AuthorData authorDTO, CancellationToken cancellationToken)
        {
            await _as.ChangeAuthor(id, authorDTO, cancellationToken);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAuthor(int id, CancellationToken cancellationToken)
        {
            await _as.DeleteAuthor(id, cancellationToken);

            return Ok();
        }
    }
}
