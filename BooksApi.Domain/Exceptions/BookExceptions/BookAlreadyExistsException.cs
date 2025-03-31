using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Domain.Exceptions.BookExceptions
{
    public class BookAlreadyExistsException : Exception
    {
        public BookAlreadyExistsException(string isbn) : base($"Книга с ISBN {isbn} уже существует.") { }
    }
}
