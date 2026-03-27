namespace CurrencyRateSample.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message, CancellationToken token);
}