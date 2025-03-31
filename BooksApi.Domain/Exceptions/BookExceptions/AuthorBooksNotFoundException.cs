using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Domain.Exceptions.BookExceptions
{
    public class AuthorBooksNotFoundException : Exception
    {
        public AuthorBooksNotFoundException(int authorId) : base($"Книги автора с id {authorId} не найдены.") { }
    }
}
