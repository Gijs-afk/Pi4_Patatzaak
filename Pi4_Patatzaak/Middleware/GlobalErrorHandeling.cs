using System.Text.Json;
using System.Net;
using System.Text.Json.Serialization;
using Pi4_Patatzaak.Exceptions;
using Serilog;

namespace Pi4_Patatzaak.Middleware
{
    public class GlobalErrorHandeling
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandeling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            var stacktrace = string.Empty;
            string message = string.Empty;

            var exceptiontype = ex.GetType();

            if (exceptiontype == typeof(Exceptions.NotFoundException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotFound;
                stacktrace = ex.StackTrace;
            }
            else if (exceptiontype == typeof(Exceptions.BadRequestException))
            {
                message = ex.Message;
                status = HttpStatusCode.BadRequest;
                stacktrace = ex.StackTrace;
            }
            else if (exceptiontype == typeof(Exceptions.NotImplementedException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotImplemented;
                stacktrace = ex.StackTrace;
            }
            else if (exceptiontype == typeof(Exceptions.UnauthorisedException))
            {
                message = ex.Message;
                status = HttpStatusCode.Unauthorized;
                stacktrace = ex.StackTrace;
            }
            else
            {
                message = ex.Message;
                status = HttpStatusCode.InternalServerError;
                stacktrace = ex.StackTrace;
            }

            var exceptionResult = JsonSerializer.Serialize(new {error = message});
            var loggerResult = JsonSerializer.Serialize(new {error = message, stacktrace});
            context.Response.ContentType= "application/json";
            context.Response.StatusCode = (int)status;

            Log.Error(loggerResult);



            return context.Response.WriteAsync(exceptionResult);
        }
    }
}
