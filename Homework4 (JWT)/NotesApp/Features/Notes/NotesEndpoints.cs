using NotesApp.Features.Helpers;
using NotesApp.Features.Notes.CreateNote;
using NotesApp.Features.Notes.DeleteNote;
using NotesApp.Features.Notes.EditNote;
using NotesApp.Features.Notes.GetNoteById;
using NotesApp.Features.Notes.GetUsersNotes;
using NotesApp.Features.Notes.Shared;

namespace NotesApp.Features.Notes;

public static class NotesEndpoints
{
    public static void MapNotes(this WebApplication app)
    {
        var group = app.MapGroup("/api/notes")
            .RequireAuthorization();
        
        group.MapGet("/", GetUsersNotesHandler.Handle);

        group.MapGet("/{id:guid}", GetNoteByIdHandler.Handle)
            .AddEndpointFilter<NoteOwnerFilter>();
        
        group.MapPost("/", CreateNoteHandler.Handle)
            .AddEndpointFilter<ValidationFilter<CreateNoteRequest>>();
        
        group.MapPut("/{id:guid}", EditNoteHandler.Handle)
            .AddEndpointFilter<NoteOwnerFilter>()
            .AddEndpointFilter<ValidationFilter<EditNoteRequest>>();

        group.MapDelete("/{id:guid}", DeleteNoteHandler.Handle)
            .AddEndpointFilter<NoteOwnerFilter>();
    }
}