using Microsoft.EntityFrameworkCore;
using NotesApp.Api.Features.Notes.Shared;
using NotesApp.Api.Persistence;

namespace NotesApp.Api.Features.Notes.GetNoteById;

public static class GetNoteByIdHandler
{
    public static async Task<IResult> Handle(
        Guid id, 
        NotesAppContext context)
    {
        var note = await context.Notes.FirstOrDefaultAsync(x => x.Id == id);
        
        if (note == null) return Results.NotFound();
        
        var userNote = new UserNote(note!.Id, note.Name, note.Content);
        
        return Results.Ok(userNote);
    }
}