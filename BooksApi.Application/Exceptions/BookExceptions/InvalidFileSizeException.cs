using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.BookExceptions
{
    public class InvalidFileSizeException : Exception
    {
        public InvalidFileSizeException() : base("Размер файла не должен превышать 1MB.") { }
    }
}
