using System.Net;

namespace BooksApi.ExceptionMiddleware
{
    public abstract class AbstractExMiddleware
    {
        private readonly RequestDelegate _next;

        public abstract (HttpStatusCode code, string message) GetResponse(Exception exception);

        public AbstractExMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                // get the response code and message
                var (status, message) = GetResponse(exception);
                response.StatusCode = (int)status;
                await response.WriteAsync(message);
            }
        }
    }

}
