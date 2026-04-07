using NotesApp.Api.Features.Notes.Shared;

namespace NotesApp.Api.Features.Notes.CreateNote;

public record CreateNoteResponse(
    Guid Id, 
    string Name, 
    string Content, 
    Guid OwnerId) 
    : UserNote(Id, Name, Content);
