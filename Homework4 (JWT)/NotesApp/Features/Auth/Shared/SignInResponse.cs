namespace NotesApp.Features.Auth.Shared;

public record SignInResponse(
    string AccessToken,
    string RefreshToken);