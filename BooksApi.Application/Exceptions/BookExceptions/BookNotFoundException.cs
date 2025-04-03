using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.BookExceptions
{
    public class BookNotFoundException : Exception
    {
        public BookNotFoundException(int id) : base($"Книга с id {id} не найдена.") { }
        public BookNotFoundException(string message) : base(message) { }
    }
}
