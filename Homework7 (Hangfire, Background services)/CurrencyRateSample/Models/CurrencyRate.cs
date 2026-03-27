namespace CurrencyRateSample.Models;

public class CurrencyRate
{
    public Guid Id { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}