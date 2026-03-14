namespace ErrorHandlingSample.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class DtoEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var request = httpContext.Request;

        if (request.ContentLength is not (null or <= 0)) return await next(context);
        
        var problemDetails = new ProblemDetails
        {
            Type = "about:blank",
            Title = "Payload is null",
            Detail = "Payload cannot be null.",
            Instance = request.Path,
            Status = StatusCodes.Status400BadRequest,
            TraceId = httpContext.TraceIdentifier,
            Timestamp = DateTimeOffset.UtcNow
        };

        return Results.BadRequest(problemDetails);

    }
}