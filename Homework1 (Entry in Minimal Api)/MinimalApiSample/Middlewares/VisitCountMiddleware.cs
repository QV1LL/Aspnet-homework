using MinimalApiSample.Services.Contracts;

namespace MinimalApiSample.Middlewares;

public class VisitCountMiddleware(RequestDelegate next)
{
    private const string VisitCountPath = "/visit-count";
    
    public async Task Invoke(HttpContext httpContext, IVisitCounter visitCounter)
    {
        visitCounter.Increase();
        Console.WriteLine($"Visited {visitCounter.Count} times");
        
        var path = httpContext.Request.Path;

        if (path != VisitCountPath)
        {
            await next(httpContext);
            return;
        }
        
        var response = httpContext.Response;
        await response.WriteAsJsonAsync(new { visitCount =  visitCounter.Count});
    }
}