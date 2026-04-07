namespace NotesApp.Api.Features.Notes.Shared;

public record UserNote(
    Guid Id, 
    string Name, 
    string Content);