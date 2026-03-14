using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ErrorHandlingSample.Orders;

public class CoffeeMachineExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not CoffeMachineException) return false;
        
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Coffee Machine Failure",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.4",
        };

        httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;

    }
}