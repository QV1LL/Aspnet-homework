namespace MinimalApiSample.Middlewares;

public class DateTimeMiddleware(RequestDelegate next)
{
    private const string DateTimePath = "/datetime";
    
    public async Task Invoke(HttpContext httpContext)
    {
        var path = httpContext.Request.Path;

        if (path != DateTimePath)
        {
            await next(httpContext);
            return;
        }
        
        var response = httpContext.Response;
        await response.WriteAsJsonAsync(new { date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
    }
}