using _2FaSample.Infrastructure.Services;
using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Features.Auth.Login;

public static class LoginHandler
{
    public static async Task<IResult> Handle(
        LoginRequest request, 
        SignInManager<AppUser> signInManager, 
        UserManager<AppUser> userManager,
        TokenService tokenService) 
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null) return Results.BadRequest("Bad credentials");

        var result = await signInManager.PasswordSignInAsync(
            user, 
            request.Password, 
            isPersistent: false,
            lockoutOnFailure: false);
        
        if (result.RequiresTwoFactor)
        {
            return Results.Ok(new 
            { 
                Requires2FA = true, 
                UserId = user.Id,
                AvailableMethods = user.EnabledTwoFactorMethods
                    .Select(m => m.ToString())
            });
        }

        if (result.Succeeded)
        {
            var token = tokenService.CreateToken(user);
            return Results.Ok(new { Token = token });
        }

        if (result.IsLockedOut) return Results.Problem("Account locked", statusCode: 423);

        return Results.BadRequest("Bad credentials");
    }
}