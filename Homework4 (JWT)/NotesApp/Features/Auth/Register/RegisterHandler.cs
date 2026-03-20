using Microsoft.EntityFrameworkCore;
using NotesApp.Features.Auth.Shared;
using NotesApp.Services.Auth;
using NotesApp.Models;
using NotesApp.Persistence;

namespace NotesApp.Features.Auth.Register;

public static class RegisterHandler
{
    public static async Task<IResult> Handle(
        RegisterRequest request,
        NotesAppContext context,
        TokenService tokenService)
    {
        if (await context.Users.AnyAsync(u => u.Name == request.Name))
        {
            return Results.BadRequest("User with this name already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        context.Users.Add(user);
        context.RefreshTokens.Add(refreshToken);
        
        await context.SaveChangesAsync();
        var accessToken = tokenService.GenerateAccessToken(user);

        return Results.Ok(new SignInResponse(accessToken, refreshToken.Token));
    }
}