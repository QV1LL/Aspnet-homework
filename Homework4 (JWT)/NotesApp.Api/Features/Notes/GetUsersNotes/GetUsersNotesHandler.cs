using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NotesApp.Api.Features.Notes.Shared;
using NotesApp.Api.Persistence;
using NotesApp.Api.Features.Helpers;

namespace NotesApp.Api.Features.Notes.GetUsersNotes;

public static class GetUsersNotesHandler
{
    public static async Task<IResult> Handle(
        NotesAppContext context, 
        ClaimsPrincipal user)
    {
        var userId = user.GetRequiredId();

        var notes = await context.Notes
            .AsNoTracking()
            .Where(n => n.OwnerId == userId)
            .Select(n => new UserNote(n.Id, n.Name, n.Content))
            .ToListAsync();
        
        return Results.Ok(notes);
    }
}