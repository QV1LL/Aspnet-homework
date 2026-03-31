namespace CachingSample.Features.Posts.AddPost;

public record Request(
    string Title, 
    string Content);