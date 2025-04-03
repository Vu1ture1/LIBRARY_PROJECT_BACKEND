using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Exceptions.UserExceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Неверный email или пароль.") { }
    }
}
