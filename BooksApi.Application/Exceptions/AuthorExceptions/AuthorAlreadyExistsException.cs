using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.AuthorExceptions
{
    public class AuthorAlreadyExistsException : Exception
    {
        public AuthorAlreadyExistsException(string name, string surname)
            : base($"Автор {name} {surname} уже существует.") { }
    }
}
