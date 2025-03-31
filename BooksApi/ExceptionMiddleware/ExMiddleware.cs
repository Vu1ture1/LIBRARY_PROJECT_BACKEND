using System.Net;
using Newtonsoft.Json;
using BooksApi.Domain.Exceptions.AuthorExceptions;
using BooksApi.Domain.Exceptions.BookExceptions;
using BooksApi.Domain.Exceptions.UserExceptions;

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
                case InvalidCredentialsException or ExpiredRefreshTokenException:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case EmailAlreadyRegisteredException or BookAlreadyExistsException or InvalidFileSizeException or InvalidFileFormatException or AuthorAlreadyExistsException or InvalidAuthorDataException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case BookNotFoundException or AuthorBooksNotFoundException or AuthorNotFoundException:
                    code = HttpStatusCode.NotFound;
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
