using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace _2FaSample.Infrastructure.Services;

public class SmsService(IConfiguration config)
{
    public async Task SendSmsAsync(string toPhoneNumber, string message)
    {
        var accountSid = config["Twilio:AccountSid"];
        var authToken = config["Twilio:AuthToken"];
        var fromNumber = config["Twilio:FromNumber"];

        TwilioClient.Init(accountSid, authToken);

        await MessageResource.CreateAsync(
            body: message,
            from: new Twilio.Types.PhoneNumber(fromNumber),
            to: new Twilio.Types.PhoneNumber(toPhoneNumber)
        );
    }
}