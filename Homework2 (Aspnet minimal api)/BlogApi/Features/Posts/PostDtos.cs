using BlogApi.Domain.Entities;

namespace BlogApi.Features.Posts;

public record CreatePost(
    string Title,
    string Content,
    List<Guid> Categories,
    List<string> Slugs);
    
public record UpdatePost(
    Guid Id,
    string Title,
    string Content,
    List<Guid> Categories,
    List<string> Slugs);
    
public record GetPosts(
    IEnumerable<Post> Items,
    int TotalCount 
);