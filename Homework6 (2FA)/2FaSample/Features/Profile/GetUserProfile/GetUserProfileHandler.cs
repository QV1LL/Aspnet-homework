using System.Security.Claims;
using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Features.Profile.GetUserProfile;

public static class GetUserProfileHandler
{
    public static async Task<IResult> Handle(
        ClaimsPrincipal principal,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.GetUserAsync(principal);

        if (user == null) return Results.Unauthorized();

        var response = new GetUserProfileResponse(
            user.UserName!,
            user.PhoneNumber,
            user.EnabledTwoFactorMethods
        );

        return Results.Ok(response);
    }
}