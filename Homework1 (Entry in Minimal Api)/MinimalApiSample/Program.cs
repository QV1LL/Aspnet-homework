using MinimalApiSample.Constants;
using MinimalApiSample.Middlewares;
using MinimalApiSample.Services.Contracts;
using MinimalApiSample.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVisitCounter, VisitCounter>();

var app = builder.Build();

app.UseMiddleware<VisitCountMiddleware>();
app.UseMiddleware<DateTimeMiddleware>();

app.MapGet("/objects", async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(HtmlConstants.ProductsMarkup);
});

app.MapGet("/mirror-headers", async (HttpContext context) =>
{
    var headers = context.Request.Headers;

    var headerItems = headers.Select(h => 
        $"<div class='header-card'>" +
        $"<span class='key'>{h.Key}</span>" +
        $"<span class='value'>{h.Value}</span>" +
        $"</div>"
    );

    var content = string.Join("", headerItems);
    var result = HtmlConstants.MirrorHeadersMarkup.Replace("{{CONTENT}}", content);

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(result);
});

app.MapGet("/profile", async (
    HttpContext context,
    string name, 
    string surname, 
    int age, 
    string? hobby, 
    string? occupation,
    string? favouriteColor) =>
{
    if (string.IsNullOrWhiteSpace(name) || 
        string.IsNullOrWhiteSpace(surname) || 
        age <= 0 || age > 120)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Invalid input: Name and Surname are required, and Age must be between 1 and 120.");
        return;
    }
    
    var layout = HtmlConstants.ProfilePageMarkup
        .Replace("{{NAME}}", name)
        .Replace("{{SURNAME}}", surname)
        .Replace("{{AGE}}", $"{age}")
        .Replace("{{HOBBY}}", hobby ?? "-")
        .Replace("{{OCCUPATION}}", occupation ?? "unemployed")
        .Replace("{{FAVOURITE_COLOR}}", favouriteColor ?? "-");
    
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(layout);
});

app.Run();
