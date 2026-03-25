using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using _2FaSample.Enums;

namespace _2FaSample.Features.Auth.TwoFactor.Totp;

public static class EnableTotpHandler
{
    public static async Task<IResult> Handle(
        EnableTotpRequest request,
        ClaimsPrincipal principal,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user, 
            userManager.Options.Tokens.AuthenticatorTokenProvider, 
            request.Code);

        if (!isValid)
            return Results.BadRequest(new { Message = "Invalid code. Please try again." });

        if (!user.EnabledTwoFactorMethods.Contains(TwoFactorMethod.Totp))
            user.EnabledTwoFactorMethods.Add(TwoFactorMethod.Totp);

        await userManager.SetTwoFactorEnabledAsync(user, true);
        await userManager.UpdateSecurityStampAsync(user);
        await userManager.UpdateAsync(user);

        return Results.Ok(new { EnabledTwoFactorMethods = user.EnabledTwoFactorMethods });
    }
}