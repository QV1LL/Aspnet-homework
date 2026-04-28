using LiqPaySample.Enums;

namespace LiqPaySample.Abstractions;

public record CreatePaymentRequest(
    Guid OrderId,
    decimal Amount,
    string? Currency,
    string? Description,
    string? IdempotencyKey = null
);

public record PaymentRequest(
    Guid PaymentId,
    decimal Amount,
    string Currency,
    string Description,
    string ReturnUrl,
    string CallbackUrl,
    string IdempotencyKey
);

public record CreatePaymentResult(
    bool Success,
    string? CheckoutUrl,    // Redirect URL для Hosted Page
    string? FormData,       // Base64 data для embedded форми (LiqPay)
    string? ProviderPaymentId,
    string? ErrorMessage
);

public record PaymentStatusResult(
    bool Success,
    PaymentStatus Status,
    string? ProviderTransactionId,
    string? ErrorMessage
);

public record RefundResult(
    bool Success,
    string? RefundId,
    string? ErrorMessage
);

public record WebhookResult(
    bool IsValid,
    PaymentStatus? NewStatus,
    string? ProviderTransactionId,
    string? ErrorMessage,
    object? RawPayload
);
