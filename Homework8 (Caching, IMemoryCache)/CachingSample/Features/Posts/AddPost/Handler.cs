using CachingSample.Models;
using CachingSample.Persistence;

namespace CachingSample.Features.Posts.AddPost;

public static class Handler
{
    public static async Task<IResult> Handle(
        Request request,
        CacheAppContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            return Results.UnprocessableEntity();

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            UpdatedAt = DateTime.UtcNow
        };
        
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        
        return Results.Created($"/posts/{post.Id}", post);
    }
}