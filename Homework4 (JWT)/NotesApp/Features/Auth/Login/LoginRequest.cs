namespace NotesApp.Features.Auth.Login;

public record LoginRequest(
    string Name, 
    string Password);