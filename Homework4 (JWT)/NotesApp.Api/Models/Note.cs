namespace NotesApp.Api.Models;

public class Note
{
    public Guid Id { get; set; }
    public required string Name  { get; set; }
    public required string Content { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
}