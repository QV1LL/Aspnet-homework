namespace BlogApi.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<string> Slugs { get; set; }
    public List<Category> Categories { get; set; }
}