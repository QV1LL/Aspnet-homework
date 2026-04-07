using NotesApp.Api.Features.Notes.Shared;

namespace NotesApp.Api.Features.Notes.GetUsersNotes;

public record UserNotesResponse(List<UserNote> Notes);