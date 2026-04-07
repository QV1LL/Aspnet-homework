using NotesApp.Api.Features.Helpers;
using NotesApp.Api.Features.Notes.CreateNote;
using NotesApp.Api.Features.Notes.DeleteNote;
using NotesApp.Api.Features.Notes.EditNote;
using NotesApp.Api.Features.Notes.GetNoteById;
using NotesApp.Api.Features.Notes.GetUsersNotes;
using NotesApp.Api.Features.Notes.Shared;

namespace NotesApp.Api.Features.Notes;

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