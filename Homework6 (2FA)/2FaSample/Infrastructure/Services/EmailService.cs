using _2FaSample.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;

namespace _2FaSample.Infrastructure.Services;

public class EmailSender(IConfiguration config) : IEmailSender<AppUser>
{
    public async Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink)
    {
        await SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    }

    public async Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink)
    {
        await SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
    }

    public async Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode)
    {
        await SendEmailAsync(email, "Reset your password", $"Your reset code is: {resetCode}");
    }

    private async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mail = config.GetSection("SmtpSettings");
        
        using var client = new SmtpClient(mail["Host"], int.Parse(mail["Port"]!))
        {
            Credentials = new NetworkCredential(mail["Username"], mail["Password"]),
            EnableSsl = true
        };

        var message = new MailMessage(mail["From"]!, email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
    }
}