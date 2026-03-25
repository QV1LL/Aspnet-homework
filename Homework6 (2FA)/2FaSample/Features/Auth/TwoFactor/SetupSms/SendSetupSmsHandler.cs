using System.Security.Claims;
using _2FaSample.Infrastructure.Services;
using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Features.Auth.TwoFactor.SetupSms;

public static class SendSetupSmsHandler
{
    public static async Task<IResult> Handler (
        UserManager<AppUser> userManager,
        SmsService smsSender,
        ClaimsPrincipal claimsUser)
    {
        var user = await userManager.GetUserAsync(claimsUser);
        if (user == null) return Results.Unauthorized();

        var phoneNumber = user.PhoneNumber;
        
        if (phoneNumber == null) return Results.BadRequest("Phone number is missing");
        
        var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        await smsSender.SendSmsAsync(phoneNumber, $"Verification code: {token}");

        return Results.Ok(new { message = "Code sent to phone number: " + phoneNumber });
    }
}