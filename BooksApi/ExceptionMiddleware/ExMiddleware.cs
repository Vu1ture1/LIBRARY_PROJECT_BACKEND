using System.Net;
using Newtonsoft.Json;
using BooksApi.CustomExceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BooksApi.ExceptionMiddleware
{
    public class ExMiddleware : AbstractExMiddleware
    {
        public ExMiddleware(RequestDelegate next) : base(next)
        {
        }

        public override (HttpStatusCode code, string message) GetResponse(Exception exception)
        {
            HttpStatusCode code;
            switch (exception)
            {
                case InvalidCredentialsException or InvalidRefreshTokenException or ExpiredRefreshTokenException or UnauthorizedUserException:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case EmailAlreadyRegisteredException or BookAlreadyExistsException or InvalidFileSizeException or InvalidFileFormatException or AuthorAlreadyExistsException or InvalidAuthorDataException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case BookNotFoundException or AuthorBooksNotFoundException or AuthorNotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;

                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }
            return (code, JsonConvert.SerializeObject(new { error = exception.Message }));
        }
    }

}
