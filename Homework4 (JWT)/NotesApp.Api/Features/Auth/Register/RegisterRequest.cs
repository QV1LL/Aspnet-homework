namespace NotesApp.Api.Features.Auth.Register;

public record RegisterRequest(
    string Name, 
    string Password);