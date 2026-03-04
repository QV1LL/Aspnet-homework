using BlogApi.Domain.Entities;
using BlogApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Posts;

public class PostService(BlogDbContext context)
{
    public async Task<List<Post>> GetPostsAsync(int page, int size, Guid? categoryId, string? searchTerm = null)
    {
        var posts = GetPostsPage(page, size, searchTerm);

        if (categoryId is not null)
        {
            posts = posts.Where(p => p.Categories.Any(c => c.Id == categoryId));
        }
        
        return await posts.ToListAsync();
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await context.Posts
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<Post?> GetPostBySlugAsync(string slug)
    {
        return await context.Posts
            .FirstOrDefaultAsync(p => p.Slugs.Contains(slug));
    }

    public async Task AddPostAsync(Post post)
    {
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
        var post = await GetPostByIdAsync(id);

        if (post is not null)
        {
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }
    }

    private IQueryable<Post> GetPostsPage(int page, int size, string? searchTerm = null)
    {
        IQueryable<Post> query = context.Posts.Include(p => p.Categories);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.Title.Contains(searchTerm) || 
                p.Content.Contains(searchTerm));
        }

        query = query.OrderByDescending(p => p.CreatedAt);

        return query
            .Skip((page - 1) * size)
            .Take(size);
    }
}