using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace _2FaSample.Features.Auth.TwoFactor.Totp;

public static class SetupTotpHandler
{
    public static async Task<IResult> Handle(
        ClaimsPrincipal principal,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        var qrUri = $"otpauth://totp/2FaSample:{user.UserName}?secret={unformattedKey}&issuer=2FaSample&digits=6";

        return Results.Ok(new 
        { 
            SharedKey = unformattedKey, 
            AuthenticatorUri = qrUri 
        });
    }
}