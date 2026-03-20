namespace NotesApp.Features.Auth.Register;

public record RegisterRequest(
    string Name, 
    string Password);