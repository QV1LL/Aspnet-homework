namespace CachingSample.Features.Posts;

public static class Endpoints
{
    public static void MapPosts(this WebApplication app)
    {
        var group = app.MapGroup("/api/posts");

        group.MapPost("/", AddPost.Handler.Handle);
        group.MapPut("/{id:guid}", UpdatePost.Handler.Handle);
        group.MapGet("/{id:guid}", GetPost.Handler.Handle);
    }
}