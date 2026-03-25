using _2FaSample.Infrastructure.Services;
using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Features.Auth.Register;

public static class RegisterHandler
{
    public static async Task<IResult> Handler (
        RegisterRequest request, 
        UserManager<AppUser> userManager,
        TokenService tokenService)
    {
        var user = new AppUser 
        { 
            UserName = request.UserName, 
            PhoneNumber = request.PhoneNumber,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Results.ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));
        }
        
        var token = tokenService.CreateToken(user);
        return Results.Ok(new { Token = token });
    }
}