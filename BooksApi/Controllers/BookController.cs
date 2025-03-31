using BooksApi.Domain.Entities;
using BooksApi.Domain.Common;
using BooksApi.Application.DTOs;
using BooksApi.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BooksApi.Domain.Exceptions.BookExceptions;
using AutoMapper;
using BooksApi.Application.Interfaces;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : Controller
    {
        private IBookService bs;
        public BookController(IBookService bs)
        {
            this.bs = bs;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetOneBookById(int id, CancellationToken cancellationToken)
        {
            var book = await bs.GetBookById(id, cancellationToken);

            return Ok(book);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<PaginatedList<Book>>> GetAllBooksWithPagination(CancellationToken cancellationToken, int pageIndex = 1, int pageSize = 4, string? cat = null, int? authorId = null)
        {
            var books = await bs.GetBooksPagList(pageIndex, pageSize, cat, authorId, cancellationToken);

            return Ok(books);
        }

        [HttpGet("isbn/{ISBN}")]
        public async Task<ActionResult<Book>> GetOneBookByISBN(string ISBN, CancellationToken cancellationToken)
        {
            var book = await bs.GetBookByISBN(ISBN, cancellationToken);
            
            return Ok(book);
        }

        [Authorize]
        [HttpGet("onhand/{id}")]
        public async Task<IActionResult> GetOneBookOnHands(int id, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(userIdString, out int userId);

            await bs.GiveBookOnHands(id, userId, cancellationToken);
            
            return Ok();
        }

        [Authorize]
        [HttpGet("onhand/user")]
        public async Task<ActionResult<List<Book>>> GetAllBooksOnHands(CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(userIdString, out int userId);

            var books = await bs.GetAllBooksOnHands(userId, cancellationToken);
            
            return Ok(books);
        }

        [HttpPost("add")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddBook([FromForm] BookData bkDTO, CancellationToken cancellationToken)
        {
            await bs.AddBook(bkDTO, cancellationToken);
            
            return Ok();
        }

        [HttpPut("change/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeBook(int id, [FromForm] BookUpdateData bkDTO, CancellationToken cancellationToken)
        {
            await bs.ChangeBook(id, bkDTO, cancellationToken);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken)
        {
            await bs.DeleteBook(id, cancellationToken);

            return NoContent();
        }

        [HttpGet("find/author/{author_id}")]
        public async Task<ActionResult<List<Book>>> FindBooksByAuthor(int author_id, CancellationToken cancellationToken)
        {
            var books = await bs.FindBooksByAuthor(author_id, cancellationToken);

            return books;
        }

        [HttpGet("find/{book_subname}")]
        public async Task<ActionResult<List<Book>>> FindBooksByName(string? book_subname, CancellationToken cancellationToken)
        {
            var books = await bs.FindBooksByName(book_subname, cancellationToken);

            return books;
        }
    }
}