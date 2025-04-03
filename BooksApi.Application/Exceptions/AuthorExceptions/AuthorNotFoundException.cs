using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.AuthorExceptions
{
    public class AuthorNotFoundException : Exception
    {
        public AuthorNotFoundException(int id) : base($"Автор с id {id} не найден.") { }
        public AuthorNotFoundException() : base("Автор не найден.") { }
    }
}
