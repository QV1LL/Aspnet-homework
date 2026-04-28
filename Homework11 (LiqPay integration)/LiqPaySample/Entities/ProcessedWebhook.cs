namespace LiqPaySample.Entities;

public class ProcessedWebhook
{
    public Guid Id { get; set; }

    // Унікальний ідентифікатор webhook від PSP
    // Для LiqPay: transaction_id
    // Для Monobank: invoiceId + status
    // Для Stripe: event.Id
    public string EventId { get; set; } = null!;

    public string Provider { get; set; } = null!;
    public DateTimeOffset ProcessedAt { get; set; }
}
