using Microsoft.EntityFrameworkCore;
using NotesApp.Features.Notes.Shared;
using NotesApp.Persistence;

namespace NotesApp.Features.Notes.GetNoteById;

public static class GetNoteByIdHandler
{
    public static async Task<IResult> Handle(
        Guid id, 
        NotesAppContext context)
    {
        var note = await context.Notes.FirstOrDefaultAsync(x => x.Id == id);
        var userNote = new UserNote(note!.Id, note.Name, note.Content);
        
        return Results.Ok(userNote);
    }
}