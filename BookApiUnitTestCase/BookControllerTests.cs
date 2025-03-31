using AutoMapper;
using BooksApi.Controllers;
using BooksApi.Domain.Exceptions.BookExceptions;
using BooksApi.Infrastructure.DbService;
using BooksApi.Domain.Entities;
using BooksApi.Application.Interfaces;
using BooksApi.Application.DTOs;
using BooksApi.Domain.Repositories;
using BooksApi.Infrastructure.SqliteRepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Application.Services;

namespace BookApiUnitTestCase
{
    public class BookControllerTests
    {
        private readonly IBookRepository bookRepository;

        private readonly AppDbContext context;

        private readonly Mock<IFileService> mock_fs;

        private readonly Mock<IMapper> mock_mapper;

        private readonly IBookService service;

        private readonly BookController bookController;

       

        private Author author = null;

        private List<Book> allBooks = null;

        public BookControllerTests() 
        {
            context = TestDbContextInMem.Create();

            author = new Author
            {
                Id = 1,
                Name = "Александр",
                Surname = "Пушкин",
                Counry = "Российская империя",
                BornDate = new DateTime(1799, 3, 21, 10, 40, 21, 749)
            };

            allBooks = new List<Book>
            {
               new Book
                {
                    ISBN = "978-5-17-057139-2",
                    Name = "Руслан и Людмила",
                    Genre = "Поэма",
                    Description = "Поэма о любви, верности и приключениях князя Руслана, который отправляется в поисках своей возлюбленной Людмилы.",
                    AuthorId = author.Id,
                    book_author = author,
                    BorrowedAt = DateTime.MinValue,
                    ReturnBy = DateTime.MinValue,
                    UserThatGetBook = 0,
                    Image = "ruslan_i_lyudmila.jpg"
                },
                new Book
                {
                    ISBN = "978-5-17-071107-6",
                    Name = "Пиковая дама",
                    Genre = "Повесть",
                    Description = "Повесть о старом расчетливом дворянине и его поисках карт, которые, по слухам, могут приносить удачу.",
                    AuthorId = author.Id,
                    book_author = author,
                    BorrowedAt = DateTime.MinValue,
                    ReturnBy = DateTime.MinValue,
                    UserThatGetBook = 0,
                    Image = "pikovaya_dama.jpg"
                },
                new Book
                {
                    ISBN = "978-5-17-050655-3",
                    Name = "Капитанская дочка",
                    Genre = "Роман",
                    Description = "Роман о судьбе молодой девушки Маши Мироновой и ее любви к Петру Гриневу, фонды которого охватывают события Пугачевского восстания.",
                    AuthorId = author.Id,
                    book_author = author,
                    BorrowedAt = DateTime.MinValue,
                    ReturnBy = DateTime.MinValue,
                    UserThatGetBook = 0,
                    Image = "kapitanskaya_dochka.jpg"
                },
                new Book
                {
                    ISBN = "978-5-17-033551-6",
                    Name = "Борис Гребенщиков",
                    Genre = "Литературный портрет",
                    Description = "Портрет знаменитого поэта Бориса Гребенщикова, одно из его знаменитых произведений.",
                    AuthorId = author.Id,
                    book_author = author,
                    BorrowedAt = DateTime.MinValue,
                    ReturnBy = DateTime.MinValue,
                    UserThatGetBook = 0,
                    Image = "boris_grebenshikov.jpg"
                }
            };

            context.AuthorsTab.AddRange(author);
            context.BooksTab.AddRange(allBooks);
            context.SaveChanges();

            bookRepository = new BookRepositorySqlite(context);

            mock_fs = new Mock<IFileService>();

            mock_mapper = new Mock<IMapper>();

            service = new BookService(bookRepository, mock_fs.Object, mock_mapper.Object);

            bookController = new BookController(service);

            var token = GenerateJwtTokenForTest("Admin", "1");

            var http_context = new DefaultHttpContext();
            http_context.Request.Headers["Authorization"] = "Bearer " + token;

            bookController.ControllerContext = new ControllerContext
            {
                HttpContext = http_context
            };

        }

        [Fact]
        public async Task AddBook_Test() 
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            
            CancellationToken token = cancelTokenSource.Token;

            var bookData = new BookData
            {
                ISBN = "978-5-17-057139-2",
                Name = "Test book",
                Genre = "Fiction",
                Description = "Some book description.",
                AuthorId = author.Id
            };

            mock_fs.Setup(fs => fs.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string[]>(), token))
                .ReturnsAsync("savedImage.jpg");

            mock_mapper.Setup(mapper => mapper.Map<Book>(It.IsAny<BookData>()))
                .Returns((BookData bookData) => new Book
                {
                   ISBN = bookData.ISBN,
                   Name = bookData.Name,
                   Genre = bookData.Genre,
                   Description = bookData.Description,
                   AuthorId = bookData.AuthorId
                });

            var result = await bookController.AddBook(bookData, token);

            var actionResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetBookById_Test()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

            CancellationToken token = cancelTokenSource.Token;

            int id = 1;

            var result = await bookController.GetOneBookById(id, token);
            
            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode);

            var returnedBook = actionResult.Value as Book;

            Assert.NotNull(returnedBook);

            Assert.Contains(returnedBook, allBooks);
        }

        [Fact]
        public async Task GetBookById_Exception_Test()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

            CancellationToken token = cancelTokenSource.Token;

            int id = 999;

            var exception = await Assert.ThrowsAsync<BookNotFoundException>(async () =>
            {
                await bookController.GetOneBookById(id, token);
            });

            // Assert
            Assert.Equal("Книга с id 999 не найдена.", exception.Message);
        }
        public string GenerateJwtTokenForTest(string role, string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aSuperSecretKeyThatIsAtLeast32CharactersLongForExample")),
                    SecurityAlgorithms.HmacSha256Signature
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }

}
