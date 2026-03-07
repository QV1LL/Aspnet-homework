using System.Text.Json.Serialization;

namespace BlogApi.Domain.Entities;

public class Category
{
    public Guid  Id { get; set; }
    public string Name { get; set; }
    
    [JsonIgnore]
    public List<Post> Posts { get; set; }
}