using CurrencyRateSample.Infrastructure.Persistence;
using CurrencyRateSample.Models;
using CurrencyRateSample.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRateSample.Services.Background;

public class EmailDispatchService(
    EmailChannel emailChannel,
    IServiceProvider provider,
    ILogger<EmailDispatchService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Email Dispatch Service is starting.");

        try
        {
            logger.LogInformation("Step 1: Reloading unsent emails from database...");
            await ReloadUnsentEmailsAsync(stoppingToken);

            logger.LogInformation("Step 2: Entering main consumption loop. Waiting for new emails...");

            await foreach (var email in emailChannel.Reader.ReadAllAsync(stoppingToken))
            {
                logger.LogDebug("Processing email ID: {Id} for {Address}", email.Id, email.To);
                
                try
                {
                    await SendAndMarkAsync(email, stoppingToken);
                    logger.LogInformation("Successfully sent email {Id} to {Address}", email.Id, email.To);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send email {Id} to {Address}", email.Id, email.To);
                    await HandleRetryAsync(email, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Email Dispatch Service is shutting down due to cancellation.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An unhandled exception occurred in the Email Dispatch Service.");
        }
        finally
        {
            logger.LogInformation("Email Dispatch Service has stopped.");
        }
    }

    private async Task ReloadUnsentEmailsAsync(CancellationToken ct)
    {
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CurrencyRateContext>();

        var unsent = await context.PendingEmails
            .Where(e => !e.IsSent)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(ct);

        foreach (var email in unsent)
        {
            await emailChannel.Writer.WriteAsync(email, ct);
        }

        logger.LogInformation("Queue populated with {Count} unsent emails from database.", unsent.Count);
    }

    private async Task SendAndMarkAsync(PendingEmail email, CancellationToken ct)
    {
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CurrencyRateContext>();
        var mailer = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var emailMessage = new EmailMessage(email.To, email.Subject, email.Body);
        
        logger.LogTrace("Attempting to call SMTP provider for email {Id}", email.Id);
        await mailer.SendEmailAsync(emailMessage, ct);

        email.IsSent = true;
        email.SentAt = DateTime.UtcNow;

        context.PendingEmails.Update(email);
        await context.SaveChangesAsync(ct);
    }

    private async Task HandleRetryAsync(PendingEmail email, CancellationToken ct)
    {
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CurrencyRateContext>();

        email.RetryCount++;

        context.PendingEmails.Update(email);
        await context.SaveChangesAsync(ct);

        if (email.RetryCount < 3)
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, email.RetryCount) * 5);
            logger.LogWarning(
                "Email {Id} failed. Attempt {Attempt}/3. Retrying in {Delay}s...",
                email.Id, email.RetryCount, delay.TotalSeconds);

            // Note: This Task.Delay will pause the entire channel consumption 
            // for this specific service instance until it finishes.
            await Task.Delay(delay, ct);
            await emailChannel.Writer.WriteAsync(email, ct);
        }
        else
        {
            logger.LogCritical(
                "Email {Id} to {Address} reached max retries and will not be re-queued.",
                email.Id, email.To);
        }
    }
}