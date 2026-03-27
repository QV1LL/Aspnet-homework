using CurrencyRateSample.Models;

namespace CurrencyRateSample.Services.Currency;

public interface ICurrencyService
{
    public Task<IEnumerable<CurrencyRate>> UpdateRatesAsync();
}