using ErrorHandlingSample.Helpers;

namespace ErrorHandlingSample.Middlewares;

using System.Net;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var traceId = context.TraceIdentifier;
        
        return context.Response.WriteAsJsonAsync(new {message = $"An internal server error occurred. Trace id: {traceId}"});
    }
}