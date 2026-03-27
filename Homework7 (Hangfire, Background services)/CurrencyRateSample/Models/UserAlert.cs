namespace CurrencyRateSample.Models;

public class UserAlert
{
    public Guid Id { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
    public string CurrencyCode { get; set; } =  string.Empty;
    public decimal TargetRate { get; set; }
    public bool IsActive { get; set; }
}