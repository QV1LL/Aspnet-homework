using LiqPaySample.Enums;

namespace LiqPaySample.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    
    // !!! Ми робимо пусту інтеграцію оплати, отже і ордера, і місця, де він буде створюватися нема.
    // public Order Order { get; set; } = null!;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "UAH";
    // ID транзакції на стороні PSP
    public string? Provider { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? Description { get; set; }

    public PaymentStatus Status { get; set; }

    // Зберігаємо raw webhook payload для аудиту
    public string? LastWebhookPayload { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // Ключ ідемпотентності
    public string IdempotencyKey { get; set; } = null!;
}
