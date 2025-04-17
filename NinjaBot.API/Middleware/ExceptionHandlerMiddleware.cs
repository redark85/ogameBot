using NinjaBot.Domain.Dtos;
using System.Net;

namespace NinjaBot.API.Middleware
{
    public class ExceptionHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e, logger);
            }
        }

        private Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ILogger logger)
        {
            logger.LogError(
                exception,
                $"{nameof(HandleExceptionAsync)}: Handling exception....");

            var response = EmptyResult.UnknownError("Unhandled exception occurred");
            if (exception is BadHttpRequestException)
            {
                response = EmptyResult.InvalidRequest(exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
#if DEBUG
                response.AppendDetails(exception.Message);
#endif
            }

            logger.LogInformation(
                $"{nameof(HandleExceptionAsync)}: The final response is going to " +
                $"be = {response.MessageId} - {response.Message}");

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
