using BooksApi.Domain.Entities;
using BooksApi.Domain.Common;
using BooksApi.Application.DTOs;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BooksApi.Application.Exceptions.BookExceptions;
using AutoMapper;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCases.UserUseCases;
using BooksApi.Application.UseCases.BookUseCases;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : Controller
    {
        public BookController(){}

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetOneBookById([FromServices] GetBookByIdUseCase use_case, int id, CancellationToken cancellationToken)
        {
            var book = await use_case.GetBookById(id, cancellationToken);

            return Ok(book);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<PaginatedList<Book>>> GetAllBooksWithPagination([FromServices] GetBooksPaginatedListUseCase use_case, CancellationToken cancellationToken, int pageIndex = 1, int pageSize = 4, string? cat = null, int? authorId = null)
        {
            var books = await use_case.GetBooksPagList(pageIndex, pageSize, cat, authorId, cancellationToken);

            return Ok(books);
        }

        [HttpGet("isbn/{ISBN}")]
        public async Task<ActionResult<Book>> GetOneBookByISBN([FromServices] GetBookByIsbnUseCase use_case, string ISBN, CancellationToken cancellationToken)
        {
            var book = await use_case.GetBookByISBN(ISBN, cancellationToken);
            
            return Ok(book);
        }

        [Authorize]
        [HttpGet("onhand/{id}")]
        public async Task<IActionResult> GiveOneBookOnHands([FromServices] GiveBookOnHandsUseCase use_case, int id, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(userIdString, out int userId);

            await use_case.GiveBookOnHands(id, userId, cancellationToken);
            
            return Ok();
        }

        [Authorize]
        [HttpGet("onhand/user")]
        public async Task<ActionResult<List<Book>>> GetAllBooksOnHands([FromServices] GetAllBooksOnHandsUseCase use_case, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(userIdString, out int userId);

            var books = await use_case.GetAllBooksOnHands(userId, cancellationToken);
            
            return Ok(books);
        }

        [HttpPost("add")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddBook([FromServices] AddBookUseCase use_case, [FromForm] BookData bkDTO, CancellationToken cancellationToken)
        {
            await use_case.AddBook(bkDTO, cancellationToken);
            
            return Ok();
        }

        [HttpPut("change/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeBook([FromServices] ChangeBookUseCase use_case, int id, [FromForm] BookUpdateData bkDTO, CancellationToken cancellationToken)
        {
            await use_case.ChangeBook(id, bkDTO, cancellationToken);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteBook([FromServices] DeleteBookUseCase use_case, int id, CancellationToken cancellationToken)
        {
            await use_case.DeleteBook(id, cancellationToken);

            return NoContent();
        }

        [HttpGet("find/author/{author_id}")]
        public async Task<ActionResult<List<Book>>> FindBooksByAuthor([FromServices] FindBooksByAuthorUseCase use_case, int author_id, CancellationToken cancellationToken)
        {
            var books = await use_case.FindBooksByAuthor(author_id, cancellationToken);

            return Ok(books);
        }

        [HttpGet("find/{book_subname}")]
        public async Task<ActionResult<List<Book>>> FindBooksByName([FromServices] FindBooksByNameUseCase use_case, string? book_subname, CancellationToken cancellationToken)
        {
            var books = await use_case.FindBooksByName(book_subname, cancellationToken);

            return Ok(books);
        }
    }
}