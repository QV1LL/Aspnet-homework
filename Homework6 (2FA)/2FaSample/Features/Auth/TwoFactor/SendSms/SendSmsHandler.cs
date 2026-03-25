using _2FaSample.Infrastructure.Services;
using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Features.Auth.TwoFactor.SendSms;

public static class SendSmsHandler
{
    public static async Task<IResult> Handle(
        SendSmsRequest request,
        UserManager<AppUser> userManager,
        SmsService smsService)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        
        if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
        {
            return Results.BadRequest("User or phone number not found");
        }

        var code = await userManager.GenerateTwoFactorTokenAsync(user, "Phone");
        
        await smsService.SendSmsAsync(user.PhoneNumber, $"Your verification code is: {code}");

        return Results.Ok();
    }
}