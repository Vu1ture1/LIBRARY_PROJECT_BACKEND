using System.Net;
using Newtonsoft.Json;
using BooksApi.Application.Exceptions.AuthorExceptions;
using BooksApi.Application.Exceptions.BookExceptions;
using BooksApi.Application.Exceptions.UserExceptions;

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
                case InvalidCredentialsException or ExpiredRefreshTokenException or UserNotFoundException:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case EmailAlreadyRegisteredException or BookAlreadyExistsException or InvalidFileSizeException or InvalidFileFormatException or AuthorAlreadyExistsException or InvalidAuthorDataException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case BookNotFoundException or AuthorBooksNotFoundException or AuthorNotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case BookOnHandsException:
                    code = HttpStatusCode.Conflict;
                    break;
                case OperationCanceledException:
                    Console.WriteLine("Операция отменена");
                    code = HttpStatusCode.RequestTimeout;
                    break;

                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }
            return (code, JsonConvert.SerializeObject(new { error = exception.Message }));
        }
    }

}
