using Microsoft.EntityFrameworkCore;
using NotesApp.Persistence;

namespace NotesApp.Features.Notes.DeleteNote;

public static class DeleteNoteHandler
{
    public static async Task<IResult> Handle(
        Guid id, 
        NotesAppContext context)
    {
        await context.Notes
            .Where(n => n.Id == id)
            .ExecuteDeleteAsync();
        
        return Results.NoContent();
    }
}