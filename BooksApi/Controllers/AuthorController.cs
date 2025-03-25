using AutoMapper;
using BooksApi.CustomExceptions;
using BooksApi.Entities;
using BooksApi.FileService;
using BooksApi.Helpers;
using BooksApi.Pagination;
using BooksApi.PostEntities;
using BooksApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository ar;

        private readonly IMapper mapper;
        public AuthorController(IAuthorRepository ar, IMapper mapper)
        {
            this.ar = ar;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TokenService.Response>> GetOneAuthorById(int id)
        {
            var author = await ar.GetAuthorById(id);

            if (author == null)
            {
                throw new AuthorNotFoundException();
            }

            return Ok(author);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Author>>> GetAllAuthorsWithPagination()
        {
            var authors = await ar.GetAllAuthors();

            return Ok(authors);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorData auth_data)
        {
            var auth = mapper.Map<Author>(auth_data);
            
            //Author auth = new Author();
            //auth.Name = auth_data.Name;
            //auth.Surname = auth_data.Surname;
            //auth.Counry = auth_data.Counry;
            //auth.BornDate = auth_data.BornDate;

            await ar.AddAuthor(auth);

            return NoContent();
        }

        [HttpPut("change/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeBook(int id, [FromBody] AuthorData auth_data)
        {
            var existingAuthor = await ar.GetAuthorById(id);

            if (existingAuthor == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound, $"Author with id: {id} does not found");
                throw new AuthorNotFoundException(id);
            }

            mapper.Map(auth_data, existingAuthor);

            //existingAuthor.Name = auth_data.Name;
            //existingAuthor.Surname = auth_data.Surname;
            //existingAuthor.Counry = auth_data.Counry;
            //existingAuthor.BornDate = auth_data.BornDate;

            await ar.ChangeAuthor(id, existingAuthor);

            return Ok();
        }
    }
}
