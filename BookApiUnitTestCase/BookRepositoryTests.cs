using BooksApi.Infrastructure.DbService;
using BooksApi.Domain.Entities;
using BooksApi.Domain.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using BooksApi.Domain.Common;
using BooksApi.Infrastructure.SqliteRepositoryServices;
using Xunit;

namespace BookApiUnitTestCase
{
    [TestCaseOrderer("TestOrderer", "BookApiUnitTestCase")]
    public class BookRepositoryTests
    {
        private readonly IBookRepository bookRepository;

        private readonly AppDbContext context;

        private Author author = null;

        private List<Book> allBooks = null;
        public BookRepositoryTests()
        {
            context = TestDbContextInMem.Create();

            this.bookRepository = new BookRepositorySqlite(context);

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

            // Arrange
            
            context.AuthorsTab.AddRange(author);
            context.BooksTab.AddRange(allBooks);
            context.SaveChanges();
        }

        [Fact, TestPriority(1)]
        public async Task AddBook_Should_Add_Book()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

            CancellationToken token = cancelTokenSource.Token;

            // Arrange
            var book = new Book
            {
                ISBN = "978-5-17-052038-2",
                Name = "Евгений Онегин",
                Genre = "Роман в стихах",
                Description = "Роман, рассказывающий о жизни молодого дворянина Евгения Онегина и его взаимоотношениях с Татьяной Лариной.",
                AuthorId = this.author.Id,
                book_author = this.author,
                BorrowedAt = DateTime.MinValue,
                ReturnBy = DateTime.MinValue,
                UserThatGetBook = 0,
                Image = "onegin.jpg"
            };

            // Act
            await bookRepository.AddBook(book, token);

            // Assert
            var addedBook = await bookRepository.GetBookByISBN("978-5-17-052038-2", token);

            Assert.NotNull(addedBook);
            Assert.Equal(book.ISBN, addedBook.ISBN);
            Assert.Equal(book.Name, addedBook.Name);
            Assert.Equal(book.Genre, addedBook.Genre);
            Assert.Equal(book.Description, addedBook.Description);
            Assert.Equal(book.AuthorId, addedBook.AuthorId);
            Assert.Equal(book.BorrowedAt, addedBook.BorrowedAt);
            Assert.Equal(book.ReturnBy, addedBook.ReturnBy);
            Assert.Equal(book.UserThatGetBook, addedBook.UserThatGetBook);
            Assert.Equal(book.Image, addedBook.Image);
        }

        [Fact, TestPriority(2)]
        public async Task GetBooks()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

            CancellationToken token = cancelTokenSource.Token;

            // Act
            var result = await bookRepository.GetBooks(1, 10, null, null, token);

            // Assert
            Assert.IsType<PaginatedList<Book>>(result);
            Assert.Equal(1, result.PageIndex);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(4, result.Items.Count);

            foreach (var book in result.Items) {
                Assert.Contains(book, allBooks);
            }
            
            TestDbContextInMem.Destroy(context);
        }
    }
}
