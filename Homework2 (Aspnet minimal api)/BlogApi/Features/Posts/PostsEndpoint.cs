using BlogApi.Domain.Entities;
using BlogApi.Features.Categories;
using BlogApi.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Posts;

public static class PostsEndpoint
{
    public static void MapPosts(this WebApplication app)
    {
        var group = app.MapGroup("/api/posts");

        group.MapGet("/", async (PostService postService, string? searchTerm, Guid? categoryId, int page = 1, int size = 10) =>
        {
            if (page < 1 || size < 1)
            {
                return Results.BadRequest("Page and Size must be greater than 0");
            }
            
            var posts = await postService.GetPostsAsync(page, size, categoryId, searchTerm);
            
            return Results.Ok(posts);
        });

        group.MapGet("/{id:Guid}", async (PostService postService, Guid id) =>
        {
            var post = await postService.GetPostByIdAsync(id);

            return post == null ? Results.NotFound("Post not found") : Results.Ok(post);
        });

        group.MapGet("/{slug}", async (PostService postService, string slug) =>
        {
            var post = await postService.GetPostBySlugAsync(slug);
            
            return post == null ? Results.NotFound("Post not found") : Results.Ok(post);
        });
        
        group.MapPost("/", async (PostService postService, CategoryService categoryService, CreatePost dto) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
            {
                return Results.UnprocessableEntity("Post title and content are required");
            }

            var categories = await categoryService.GetCategoriesByIdsAsync(dto.Categories);
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Slugs = dto.Slugs,
                Categories = categories
            };
            await postService.AddPostAsync(post);
            
            return Results.Created($"/posts/{post.Id}", post);
        });

        group.MapPut("/{id:Guid}/attachments", async (
            PostService postService, 
            IFileService fileService, 
            Guid id, 
            [FromForm]IFormFile? imageFile) =>
        {
            if (imageFile is null)
            {
                return Results.UnprocessableEntity("File is empty");
            }
            
            var post =  await postService.GetPostByIdAsync(id);

            if (post is null)
            {
                return Results.NotFound("Post not found");
            }
            
            post.ImageUrl = await fileService.UploadFileAsync(imageFile);
            await postService.UpdatePostAsync(post);
            
            return Results.NoContent();
        });
        
        group.MapPut("/", async (PostService postService, CategoryService categoryService, UpdatePost dto) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
            {
                return Results.UnprocessableEntity("Post title and content are required");
            }

            var existingPost = await postService.GetPostByIdAsync(dto.Id);
            if (existingPost is null)
            {
                return Results.NotFound("Post not found");
            }

            existingPost.Title = dto.Title;
            existingPost.Content = dto.Content;
            existingPost.Slugs = dto.Slugs;

            if (dto.Categories.Count > 0)
            {
                existingPost.Categories = await categoryService.GetCategoriesByIdsAsync(dto.Categories);
            }

            await postService.UpdatePostAsync(existingPost);

            return Results.NoContent();
        });

        group.MapDelete("/{id:Guid}", async (PostService postService, Guid id) =>
        {
            var post = await postService.GetPostByIdAsync(id);
            
            if (post is null)
            {
                return Results.NotFound("Post not found");
            }

            await postService.DeletePostAsync(id);

            return Results.NoContent();
        });
    }
}