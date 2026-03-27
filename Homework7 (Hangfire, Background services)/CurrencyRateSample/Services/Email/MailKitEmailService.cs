using MailKit.Net.Smtp;
using MimeKit;

namespace CurrencyRateSample.Services.Email;

public class MailKitEmailService(IConfiguration config) : IEmailService
{
    public async Task SendEmailAsync(EmailMessage message, CancellationToken token)
    {
        var mimeMessage = new MimeMessage();
        
        mimeMessage.From.Add(new MailboxAddress(
            config["EmailSettings:SenderName"], 
            config["EmailSettings:SenderEmail"]!));
            
        mimeMessage.To.Add(new MailboxAddress("", message.To));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart("html") { Text = message.Body };

        using var client = new SmtpClient();
        
        await client.ConnectAsync(
            config["EmailSettings:SmtpServer"], 
            int.Parse(config["EmailSettings:Port"] ?? "587"), 
            MailKit.Security.SecureSocketOptions.StartTls,
            token);

        if (!string.IsNullOrEmpty(config["EmailSettings:Username"]))
        {
            await client.AuthenticateAsync(
                config["EmailSettings:Username"], 
                config["EmailSettings:Password"],
                token);
        }

        await client.SendAsync(mimeMessage, token);
        await client.DisconnectAsync(true, token);
    }
}