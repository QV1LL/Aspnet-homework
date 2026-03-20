using NotesApp.Features.Notes.Shared;

namespace NotesApp.Features.Notes.GetUsersNotes;

public record UserNotesResponse(List<UserNote> Notes);