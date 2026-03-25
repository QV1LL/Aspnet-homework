using System.Security.Claims;
using _2FaSample.Enums;
using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Features.Auth.TwoFactor.SetupSms;

public static class EnableSmsTwoFactor
{
    public static async Task<IResult> Handle(
        EnableSmsTwoFactorRequest request,
        UserManager<AppUser> userManager,
        ClaimsPrincipal claimsUser)
    {
        var user = await userManager.GetUserAsync(claimsUser);
        if (user == null) return Results.Unauthorized();

        var result = await userManager.ChangePhoneNumberAsync(user, user.PhoneNumber ?? string.Empty, request.Code);
        if (!result.Succeeded) 
            return Results.BadRequest("Wrong or expired code");

        if (!user.EnabledTwoFactorMethods.Contains(TwoFactorMethod.Sms))
            user.EnabledTwoFactorMethods.Add(TwoFactorMethod.Sms);

        await userManager.SetTwoFactorEnabledAsync(user, true);
        await userManager.UpdateSecurityStampAsync(user);
        await userManager.UpdateAsync(user);

        return Results.Ok(new { EnabledTwoFactorMethods = user.EnabledTwoFactorMethods }); 
    }
}
