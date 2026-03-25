using _2FaSample.Enums;
using _2FaSample.Infrastructure.Services;
using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Features.Auth.TwoFactor.Verify;

public static class VerifyTwoFactorHandler
{
    public static async Task<IResult> Handle(
        VerifyTwoFactorRequest request,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        TokenService tokenService)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null) return Results.Unauthorized();

        var provider = request.Method switch
        {
            "Sms" => "Phone",
            "Totp" => "Authenticator",
            _ => throw new ArgumentOutOfRangeException()
        };

        var isValid = await userManager.VerifyTwoFactorTokenAsync(user, provider, request.Code);

        if (!isValid)
        {
            return Results.BadRequest(new { Message = "Invalid or expired code" });
        }

        var token = tokenService.CreateToken(user);
        
        return Results.Ok(new { Token = token });
    }
}