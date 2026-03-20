using Microsoft.EntityFrameworkCore;
using NotesApp.Features.Auth.Shared;
using NotesApp.Services.Auth;
using NotesApp.Persistence;

namespace NotesApp.Features.Auth.Login;

public static class LoginHandler
{
    public static async Task<IResult> Handle(
        LoginRequest request,
        NotesAppContext context,
        TokenService tokenService)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Name == request.Name);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Results.Unauthorized();
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshTokenEntity = tokenService.CreateRefreshToken(user.Id);

        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync();

        var response = new SignInResponse(accessToken, refreshTokenEntity.Token);
        
        return Results.Ok(response);
    }
}