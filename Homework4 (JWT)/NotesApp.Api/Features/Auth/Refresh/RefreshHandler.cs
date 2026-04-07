using Microsoft.EntityFrameworkCore;
using NotesApp.Api.Features.Auth.Shared;
using NotesApp.Api.Persistence;
using NotesApp.Api.Services.Auth;

namespace NotesApp.Api.Features.Auth.Refresh;

public static class RefreshHandler
{
    public static async Task<IResult> Handle(
        RefreshRequest request,
        NotesAppContext context,
        TokenService tokenService)
    {
        var existingToken = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            return Results.Unauthorized();
        }

        existingToken.IsRevoked = true;

        var user = existingToken.User;
        var newAccessToken = tokenService.GenerateAccessToken(user);
        var newRefreshTokenEntity = tokenService.CreateRefreshToken(user.Id);

        context.RefreshTokens.Add(newRefreshTokenEntity);
        await context.SaveChangesAsync();

        return Results.Ok(new SignInResponse(newAccessToken, newRefreshTokenEntity.Token));
    }
}