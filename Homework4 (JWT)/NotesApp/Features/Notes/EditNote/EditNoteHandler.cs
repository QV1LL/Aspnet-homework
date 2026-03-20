using Microsoft.EntityFrameworkCore;
using NotesApp.Persistence;

namespace NotesApp.Features.Notes.EditNote;

public static class EditNoteHandler
{
    public static async Task<IResult> Handle(
        Guid id, 
        EditNoteRequest request, 
        NotesAppContext context)
    {
        var rowsAffected = await context.Notes
            .Where(n => n.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Name, request.Name)
                .SetProperty(n => n.Content, request.Content));

        return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
    }
}