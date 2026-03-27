using CurrencyRateSample.Infrastructure.Persistence;
using CurrencyRateSample.Models;
using CurrencyRateSample.Services.Currency;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRateSample.Services.Background;

public class CurrencyMonitoringService(
    EmailChannel emailChannel,
    IConfiguration configuration,
    ILogger<CurrencyMonitoringService> logger,
    IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!int.TryParse(configuration["CurrenciesRateService:PeriodInSeconds"], out var seconds))
            throw new ArgumentException("CurrenciesRateService:PeriodInSeconds must be a number");
        
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(seconds));
        await using var scope = provider.CreateAsyncScope();
        var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();
        
        while (!stoppingToken.IsCancellationRequested && 
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var newCurrenciesRates = await currencyService.UpdateRatesAsync();
                await AddPendingEmails(newCurrenciesRates);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while monitoring currencies");
            }
        }
    }

    private async Task AddPendingEmails(IEnumerable<CurrencyRate> newCurrenciesRates)
    {
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CurrencyRateContext>();

        var rateMap = newCurrenciesRates.ToDictionary(r => r.CurrencyCode, r => r.Rate);

        var triggeredAlerts = await context.UserAlerts
            .Include(a => a.User)
            .Where(a => a.IsActive && rateMap.Keys.Contains(a.CurrencyCode))
            .ToListAsync();

        var alertsHit = triggeredAlerts
            .Where(a => rateMap[a.CurrencyCode] >= a.TargetRate)
            .ToList();

        if (alertsHit.Count == 0) 
            return;

        var pendingEmails = alertsHit.Select(a =>
        {
            var currentRate = rateMap[a.CurrencyCode];

            return new PendingEmail
            {
                To      = a.User!.Email,
                Subject = $"Rate alert triggered for {a.CurrencyCode}",
                Body    = $"Your target rate of {a.TargetRate:F4} for {a.CurrencyCode} " +
                          $"has been reached. Current rate: {currentRate:F4}."
            };
        }).ToList();

        // Just for demo we donno turn it off, so emails will sends often
        // foreach (var alert in alertsHit)
        //     alert.IsActive = false;

        context.PendingEmails.AddRange(pendingEmails);
        await context.SaveChangesAsync();

        foreach (var email in pendingEmails)
        {
            logger.LogInformation("Email triggered for {@context}", email);
            await emailChannel.Writer.WriteAsync(email);
        }
    }
}