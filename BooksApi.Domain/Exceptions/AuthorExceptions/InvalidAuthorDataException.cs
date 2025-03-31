using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Domain.Exceptions.AuthorExceptions
{
    public class InvalidAuthorDataException : Exception
    {
        public InvalidAuthorDataException() : base("Предоставлены недействительные данные об авторе.") { }
    }
}
