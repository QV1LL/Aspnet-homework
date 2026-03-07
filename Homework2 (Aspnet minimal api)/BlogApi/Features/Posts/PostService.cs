using BlogApi.Domain.Entities;
using BlogApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Posts;

public class PostService(BlogDbContext context)
{
    public async Task<GetPosts> GetPostsAsync(int page, int size, Guid? categoryId, string? searchTerm = null)
    {
        var query = context.Posts
            .Include(p => p.Categories)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(p => 
                p.Title.ToLower().Contains(searchLower) || 
                p.Content.ToLower().Contains(searchLower));
        }

        if (categoryId is not null && categoryId != Guid.Empty)
        {
            query = query.Where(p => p.Categories.Any(c => c.Id == categoryId));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new GetPosts(items, totalCount);
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await context.Posts
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post?> GetPostBySlugAsync(string slug)
    {
        return await context.Posts
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Slugs.Any(s => s == slug));
    }

    public async Task AddPostAsync(Post post)
    {
        post.ImageUrl ??= "";
        post.CreatedAt = DateTime.UtcNow;
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        context.Entry(post).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await context.Posts.FindAsync(id);
        if (post is not null)
        {
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }
    }
}