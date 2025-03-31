using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Domain.Responses
{
    public record ResponseForCookie(string AccessToken, string RefreshToken, DateTime ExpiredDate);
}