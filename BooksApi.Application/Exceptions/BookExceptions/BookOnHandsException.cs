using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.BookExceptions
{
    public class BookOnHandsException : Exception
    {
        public BookOnHandsException() : base("Данная книга уже занята пользователя.") { }
    }
}
