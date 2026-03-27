using CurrencyRateSample.Infrastructure.Persistence;
using CurrencyRateSample.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRateSample.Services.Currency;

public class MockCurrencyService(CurrencyRateContext context) : ICurrencyService
{
    private readonly Random _random = new();

    public async Task<IEnumerable<CurrencyRate>> UpdateRatesAsync()
    {
        var currentRates = await context.CurrencyRates.ToListAsync();

        var updatedRates = currentRates
            .Select(rate =>
            {
                var fluctuation = (decimal)(_random.NextDouble() * 0.02 - 0.01);

                rate.Rate = Math.Round(rate.Rate * (1 + fluctuation), 4);
                rate.LastUpdated = DateTimeOffset.UtcNow;

                return rate;
            })
            .ToList();

        await context.SaveChangesAsync();

        return updatedRates;
    }
}