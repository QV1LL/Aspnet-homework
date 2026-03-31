using CachingSample.Features.Posts.Shared;
using CachingSample.Models;
using CachingSample.Persistence;
using Microsoft.Extensions.Caching.Memory;

namespace CachingSample.Features.Posts.UpdatePost;

public static class Handler
{
    public static async Task<IResult> Handle(
        Guid id,
        CacheAppContext context,
        Request request,
        IMemoryCache cache,
        ILogger<Post> logger)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            return Results.UnprocessableEntity();
        
        var post = await context.Posts.FindAsync(id);

        if (post == null)
        {
            logger.LogWarning("Update failed: Post {PostId} not found.", id);
            return Results.NotFound();
        }

        post.Title = request.Title;
        post.Content = request.Content;
        
        await context.SaveChangesAsync();

        var cacheKey = $"{ServicesConsts.CacheKey}{post.Id}";
        cache.Remove(cacheKey);
        
        logger.LogInformation("Post {PostId} updated in database and evicted from cache.", id);
        
        return Results.NoContent();
    }
}