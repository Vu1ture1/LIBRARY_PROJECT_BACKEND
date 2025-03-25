using BooksApi.DbService;
using BooksApi.Entities;
using BooksApi.Helpers;
using BooksApi.Pagination;
using BooksApi.PostEntities;
using BooksApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using BooksApi.FileService;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using BooksApi.CustomExceptions;
using AutoMapper;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : Controller
    {
        private readonly IBookRepository br;

        private IFileService fs;

        private readonly IMapper mapper;
        public BookController(IBookRepository br, IFileService fs, IMapper mapper)
        {
            this.br = br;
            this.fs = fs;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetOneBookById(int id)
        {
            var book = await br.GetBookById(id);

            if (book == null) {
                throw new BookNotFoundException(id);
            }

            return Ok(book);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<PaginatedList<Book>>> GetAllBooksWithPagination(int pageIndex = 1, int pageSize = 4, string? cat = null, int? authorId = null)
        {
            var books = await br.GetBooks(pageIndex, pageSize, cat, authorId);

            return Ok(books);
        }

        [HttpGet("isbn/{ISBN}")]
        public async Task<ActionResult<Book>> GetOneBookByISBN(string ISBN)
        {
            var book = await br.GetBookByISBN(ISBN);

            if (book == null) 
            { 
                throw new BookNotFoundException($"Книги с таким isbn: {ISBN} нет.");
            }
            
            return Ok(book);
        }

        [Authorize]
        [HttpGet("onhand/{id}")]
        public async Task<IActionResult> GetOneBookOnHands(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(userIdString, out int userId);

            await br.GiveBookToUser(id, userId);
            
            return NoContent();
        }

        [Authorize]
        [HttpGet("onhand/user")]
        public async Task<ActionResult<List<Book>>> GetAllBooksOnHands()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(userIdString, out int userId);
            
            var books = await br.FindUserBooksOnHands(userId);

            if (books == null) {
                //return NotFound(new { message =  });
                throw new BookNotFoundException("Пользователь не забирал никаких книг.");
            }

            return Ok(books);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook([FromForm] BookData bk_data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (bk_data.ImageFile != null && bk_data.ImageFile?.Length > 1 * 1024 * 1024)
            {
                //return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
                throw new InvalidFileSizeException();
            }

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

            string createdImageName = await fs.SaveFileAsync(bk_data.ImageFile, allowedFileExtentions);

            var book = mapper.Map<Book>(bk_data);

            //Book book = new Book();

            //book.ISBN = bk_data.ISBN;
            //book.Name = bk_data.Name;
            //book.Genre = bk_data.Genre;
            //book.Description = bk_data.Description;

            //book.AuthorId = bk_data.AuthorId;

            book.Image = createdImageName;

            await br.AddBook(book);

            return NoContent();
        }

        [HttpPut("change/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeBook(int id, [FromForm] BookUpdateData bk)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = await br.GetBookById(id);

            if (existingBook == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound, $"Book with id: {id} does not found");
                throw new BookNotFoundException(id);
            }

            string oldImage = existingBook.Image;

            if (bk.ImageFile != null)
            {
                if (bk.ImageFile?.Length > 1 * 1024 * 1024)
                {
                    //return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
                    throw new InvalidFileSizeException();
                }

                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

                string createdImageName = await fs.SaveFileAsync(bk.ImageFile, allowedFileExtentions);

                bk.Image = createdImageName;
            }
            else
            {
                bk.Image = existingBook.Image;
            }

            mapper.Map(bk, existingBook);

            //existingBook.ISBN = bk.ISBN;
            //existingBook.Name = bk.Name;
            //existingBook.Genre = bk.Genre;
            //existingBook.Description = bk.Description;

            //existingBook.AuthorId = bk.AuthorId;

            //existingBook.Image = bk.Image;

            await br.ChangeBook(id, existingBook);

            if (bk.ImageFile != null && oldImage != "Default.jpg")
                fs.DeleteFile(oldImage);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var existingBook = await br.GetBookById(id);

            if (existingBook == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound, $"Book with id: {id} does not found");
                throw new BookNotFoundException(id);
            }

            await br.RemoveBook(id);

            if (existingBook.Image != "Default.jpg")
            {
                fs.DeleteFile(existingBook.Image);
            }

            return NoContent();  // return 204
        }

        [HttpGet("find/author/{author_id}")]
        public async Task<ActionResult<List<Book>>> FindBooksByAuthor(int author_id)
        {
            var books = await br.FindBooksByAuthor(author_id);

            if (books == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound, $"Books with this author: {author_id} does not found");
                throw new AuthorBooksNotFoundException(author_id);
            }

            return books;
        }

        [HttpGet("find/{book_subname}")]
        public async Task<ActionResult<List<Book>>> FindBooksByName(string? book_subname)
        {
            if (book_subname == null) {
                throw new BookNotFoundException($"Книг с указанной подстрокой имени: null не найдено.");
            }
            
            var books = await br.FindBookBySubstringName(book_subname);

            if (books == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound, $"Books does not found");
                throw new BookNotFoundException($"Книг с указанной подстрокой имени: {book_subname} не найдено.");
            }

            return books;
        }
    }
}

//9783752994285
//Дубровский
//Роман 
//Наиболее известный разбойничий роман на русском языке, необработанное для печати (и неоконченное) произведение А. С. Пушкина. Повествует о любви Владимира Дубровского к Марии Троекуровой — потомков двух враждующих помещичьих семейств.
//1

//http://localhost:5002/api/books/isbn/9783752994285

//{ "accessToken":"eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIyIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoibWlzaGFAZ21haWwuY29tIiwibmJmIjoxNzQyNTAxNTE0LCJleHAiOjE3NDI1MDIxMTR9.DkDqJwV1IecP18JT-tj23uMkR8-srYo4lUgDRsSHarI","refreshToken":"SItmQiJZKzEpzo+JnrEiY3izGEMoJYqfcORhp5dSlYY="}