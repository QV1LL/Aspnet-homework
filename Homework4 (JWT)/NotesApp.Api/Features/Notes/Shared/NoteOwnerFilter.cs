using System.Security.Claims;
using NotesApp.Api.Persistence;
using NotesApp.Api.Features.Helpers;

namespace NotesApp.Api.Features.Notes.Shared;

public class NoteOwnerFilter(NotesAppContext dbContext) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        var noteId = context.GetArgument<Guid>(0);
        var userId = context.HttpContext.User.GetId();
        
        var note = await dbContext.Notes.FindAsync(noteId);

        if (note is null)
        {
            return Results.NotFound("Note not found");
        }

        if (note.OwnerId != userId)
        {
            return Results.Forbid();
        }
        
        return await next(context);
    }
}