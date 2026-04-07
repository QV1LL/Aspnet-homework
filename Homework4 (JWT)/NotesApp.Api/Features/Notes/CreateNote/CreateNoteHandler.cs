using System.Security.Claims;
using NotesApp.Api.Models;
using NotesApp.Api.Persistence;
using NotesApp.Api.Features.Helpers;

namespace NotesApp.Api.Features.Notes.CreateNote;

public static class CreateNoteHandler
{
    public static async Task<IResult> Handle(
        CreateNoteRequest request, 
        NotesAppContext context, 
        ClaimsPrincipal userPrincipal)
    {
        var userId = userPrincipal.GetRequiredId();
        var user = await context.Users.FindAsync(userId);
        
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Content = request.Content,
            Owner = user!,
            OwnerId = user!.Id
        };
        
        await context.Notes.AddAsync(note);
        await context.SaveChangesAsync();
        
        var response = new CreateNoteResponse(
            note.Id, note.Name, note.Content, note.OwnerId);
        
        return Results.Created($"/notes/{note.Id}", response);
    }
}