using _2FaSample.Features.Auth.Login;
using _2FaSample.Features.Auth.Register;
using _2FaSample.Features.Auth.TwoFactor.SendSms;
using _2FaSample.Features.Auth.TwoFactor.SetupSms;
using _2FaSample.Features.Auth.TwoFactor.Totp;
using _2FaSample.Features.Auth.TwoFactor.Verify;

namespace _2FaSample.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api")
            .WithTags("Auth");
        
        group.MapPost("/register", RegisterHandler.Handler);
        group.MapPost("/login", LoginHandler.Handle);

        var twoFactorGroup = group
            .MapGroup("/2fa");
        
        twoFactorGroup.MapPost("/sms/setup", SendSetupSmsHandler.Handler)
            .RequireAuthorization();
        twoFactorGroup.MapPost("/sms/enable", EnableSmsTwoFactor.Handle)
            .RequireAuthorization();
        twoFactorGroup.MapPost("/sms/send-login-code", SendSmsHandler.Handle);

        twoFactorGroup.MapPost("/totp/setup", SetupTotpHandler.Handle)
            .RequireAuthorization();
        twoFactorGroup.MapPost("/totp/enable", EnableTotpHandler.Handle)
            .RequireAuthorization();

        twoFactorGroup.MapPost("/verify", VerifyTwoFactorHandler.Handle);
    }
}