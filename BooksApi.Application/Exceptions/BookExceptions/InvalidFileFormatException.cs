using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.BookExceptions
{
    public class InvalidFileFormatException : Exception
    {
        public InvalidFileFormatException() : base("Неверный формат файла. Разрешены только .jpg, .jpeg, .png.") { }
    }
}
