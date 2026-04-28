namespace LiqPaySample.Abstractions;

public interface IPaymentProvider
{
    // Унікальна назва провайдера: "liqpay", "monobank", "stripe"
    string ProviderName { get; }

    /// <summary>
    /// Ініціює платіж. Повертає URL для redirect або токен форми.
    /// </summary>
    Task<CreatePaymentResult> CreatePaymentAsync(
        PaymentRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Перевіряє поточний статус транзакції на стороні PSP.
    /// </summary>
    Task<PaymentStatusResult> GetPaymentStatusAsync(
        string providerTransactionId,
        CancellationToken ct = default);

    /// <summary>
    /// Ініціює повернення коштів (повне або часткове).
    /// </summary> 
    Task<RefundResult> RefundAsync(
        string providerTransactionId,
        decimal amount,
        CancellationToken ct = default);

    /// <summary>
    /// Верифікує підпис вхідного webhook та парсить payload.
    /// </summary>
    Task<WebhookResult> ProcessWebhookAsync(
        HttpRequest request,
        CancellationToken ct = default);
}
