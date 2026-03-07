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
            new() { Id = Guid.NewGuid(), Name = "Programming" },
            new() { Id = Guid.NewGuid(), Name = "Lifestyle" },
            new() { Id = Guid.NewGuid(), Name = "DevOps" }
        };

        var posts = new List<Post>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Navigating Docker with Captains",
                Content = "Detailed guide about Docker containers and how to manage them effectively in production environments.",
                ImageUrl = "https://images.unsplash.com/photo-1605745341112-85968b193ef5",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                Slugs = new List<string> { "navigating-docker", "docker-captains" },
                Categories = new List<Category> { categories[1], categories[4] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Clean Architecture in .NET",
                Content = "How to build maintainable applications using Clean Architecture principles in the .NET ecosystem.",
                ImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c",
                CreatedAt = DateTime.UtcNow.AddDays(-9),
                Slugs = new List<string> { "clean-architecture-dotnet" },
                Categories = new List<Category> { categories[0], categories[2] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Mastering React Hooks",
                Content = "A deep dive into useEffect, useMemo, and custom hooks for better state management in React.",
                ImageUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                Slugs = new List<string> { "mastering-react-hooks" },
                Categories = new List<Category> { categories[0], categories[2] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Kubernetes for Beginners",
                Content = "Everything you need to know to start orchestrating containers with Kubernetes from scratch.",
                ImageUrl = "https://images.unsplash.com/photo-1667372393119-3d4c48d07fc9",
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                Slugs = new List<string> { "kubernetes-beginners" },
                Categories = new List<Category> { categories[1], categories[4] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "The Rise of AI in 2026",
                Content = "Exploring how artificial intelligence is reshaping the software development industry this year.",
                ImageUrl = "https://images.unsplash.com/photo-1677442136019-21780ecad995",
                CreatedAt = DateTime.UtcNow.AddDays(-6),
                Slugs = new List<string> { "ai-rise-2026" },
                Categories = new List<Category> { categories[0] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Morning Routine for Developers",
                Content = "How starting your day with deep work and proper nutrition can double your productivity.",
                ImageUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Slugs = new List<string> { "dev-morning-routine" },
                Categories = new List<Category> { categories[3] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Entity Framework Core Tips",
                Content = "Optimization techniques for EF Core to handle complex queries without losing performance.",
                ImageUrl = "https://images.unsplash.com/photo-1544383835-bda2bc66a55d",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                Slugs = new List<string> { "ef-core-tips" },
                Categories = new List<Category> { categories[2] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "CI/CD Pipelines with GitHub Actions",
                Content = "Automating your deployment workflow has never been easier with GitHub Actions and YAML.",
                ImageUrl = "https://images.unsplash.com/photo-1618401471353-b98aadebc256",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Slugs = new List<string> { "github-actions-cicd" },
                Categories = new List<Category> { categories[4] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "TypeScript vs JavaScript",
                Content = "Why static typing is becoming the standard for large-scale web applications in 2026.",
                ImageUrl = "https://images.unsplash.com/photo-1516116216624-53e697fedbea",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Slugs = new List<string> { "typescript-vs-javascript" },
                Categories = new List<Category> { categories[0], categories[2] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Microservices Patterns",
                Content = "Learning about API Gateways, Service Discovery, and Event-Driven architecture.",
                ImageUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Slugs = new List<string> { "microservices-patterns" },
                Categories = new List<Category> { categories[0], categories[4] }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Sustainable Coding Practices",
                Content = "How to write code that is not only clean but also energy-efficient for a better future.",
                ImageUrl = "https://images.unsplash.com/photo-1497215728101-856f4ea42174",
                CreatedAt = DateTime.UtcNow,
                Slugs = new List<string> { "sustainable-coding" },
                Categories = new List<Category> { categories[2], categories[3] }
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.Posts.AddRangeAsync(posts);
        await context.SaveChangesAsync();
    }
}