using CachingSample.Features.Posts.Shared;
using CachingSample.Models;
using CachingSample.Persistence;
using Microsoft.Extensions.Caching.Memory;

namespace CachingSample.Features.Posts.GetPost;

public static class Handler
{
    public static async Task<IResult> Handle(
        Guid id,
        IMemoryCache cache,
        CacheAppContext context,
        ILogger<Post> logger)
    {
        var cacheKey = $"{ServicesConsts.CacheKey}{id}";
        
        if (cache.TryGetValue(cacheKey, out Post? post))
        {
            logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return Results.Ok(post);
        }

        logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from database.", cacheKey);
        
        var fetchedPost = await context.Posts.FindAsync(id);
        
        if (fetchedPost == null)
        {
            logger.LogWarning("Post with ID {PostId} not found.", id);
            return Results.NotFound();
        }
        
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
            .SetPriority(CacheItemPriority.Normal);
        cache.Set(cacheKey, fetchedPost, cacheOptions);
        
        logger.LogInformation("Post {PostId} added to cache.", id);
        
        return Results.Ok(fetchedPost);
    }
}