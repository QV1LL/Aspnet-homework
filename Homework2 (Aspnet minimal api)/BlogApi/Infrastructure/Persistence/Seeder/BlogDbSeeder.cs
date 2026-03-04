using BlogApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Infrastructure.Persistence.Seeder;

public static class BlogDbSeeder
{
    public static async Task SeedAsync(BlogDbContext context)
    {
        if (await context.Categories.AnyAsync() || await context.Posts.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Technology" },
            new() { Id = Guid.NewGuid(), Name = "Docker" },
            new() { Id = Guid.NewGuid(), Name = "Programming" }
        };

        var posts = new List<Post>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Navigating Docker with Captains",
                Content = "Detailed guide about Docker containers...",
                ImageUrl = "https://example.com/docker.png",
                CreatedAt = DateTime.UtcNow,
                Slugs = new List<string> { "navigating-docker", "docker-captains" },
                Categories = new List<Category> { categories[1], categories[2] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Clean Architecture in .NET",
                Content = "How to build maintainable applications...",
                ImageUrl = "https://example.com/dotnet.png",
                CreatedAt = DateTime.UtcNow,
                Slugs = new List<string> { "clean-architecture-dotnet" },
                Categories = new List<Category> { categories[0], categories[2] }
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.Posts.AddRangeAsync(posts);
        await context.SaveChangesAsync();
    }
}