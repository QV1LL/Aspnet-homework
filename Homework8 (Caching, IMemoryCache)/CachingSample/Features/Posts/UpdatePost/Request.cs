namespace CachingSample.Features.Posts.UpdatePost;

public record Request(
    string Title, 
    string Content);