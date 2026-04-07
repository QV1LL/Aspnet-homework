namespace NotesApp.Api.Features.Auth.Login;

public record LoginRequest(
    string Name, 
    string Password);