using LiqPaySample.Abstractions;
using LiqPaySample.Enums;

namespace LiqPaySample.Providers.LiqPay;

public class LiqPayPaymentProvider(LiqPayClient client) : IPaymentProvider
{
    public string ProviderName => "liqpay";

    public Task<CreatePaymentResult> CreatePaymentAsync(
        PaymentRequest request,
        CancellationToken ct = default)
    {
        // Формуємо payload для LiqPay Checkout
        var payload = new
        {
            version = 3,
            public_key = client.PublicKey,
            action = "pay",             // тип операції: оплата
            amount = request.Amount,
            currency = request.Currency,
            description = request.Description,
            order_id = request.PaymentId.ToString(),
            result_url = request.ReturnUrl,
            server_url = request.CallbackUrl,
            sandbox = client.IsSandbox ? 1 : 0
        };

        // Отримуємо закодовані параметри для форми
        var (data, signature) = client.GetCheckoutParams(payload);

        // Формуємо URL checkout-сторінки LiqPay
        var checkoutUrl =
            $"{client.CheckoutBaseUrl}?data={Uri.EscapeDataString(data)}" +
            $"&signature={Uri.EscapeDataString(signature)}";

        var result = new CreatePaymentResult(
            Success: true,
            CheckoutUrl: checkoutUrl,
            FormData: data,         // Також повертаємо для embedded виджету
            ProviderPaymentId: null, // LiqPay не повертає ID до редиректу
            ErrorMessage: null
        );

        return Task.FromResult(result);
    }

    public async Task<PaymentStatusResult> GetPaymentStatusAsync(
        string providerTransactionId,
        CancellationToken ct = default)
    {
        var payload = new
        {
            version = 3,
            public_key = client.PublicKey,
            action = "status",
            order_id = providerTransactionId
        };

        var response = await client.SendApiRequestAsync(payload, ct);

        if (response is null)
            return new PaymentStatusResult(false, PaymentStatus.Failed, null,
                "Empty response from LiqPay");

        response.TryGetValue("status", out var status);
        response.TryGetValue("transaction_id", out var txId);

        return new PaymentStatusResult(
            Success: true,
            Status: MapLiqPayStatus(status),
            ProviderTransactionId: txId,
            ErrorMessage: null
        );
    }

    public async Task<RefundResult> RefundAsync(
        string providerTransactionId,
        decimal amount,
        CancellationToken ct = default)
    {
        var payload = new
        {
            version = 3,
            public_key = client.PublicKey,
            action = "refund",
            order_id = providerTransactionId,
            amount = amount
        };

        var response = await client.SendApiRequestAsync(payload, ct);

        if (response is null)
            return new RefundResult(false, null, "Empty response from LiqPay");

        response.TryGetValue("result", out var result);
        response.TryGetValue("transaction_id", out var txId);

        return result == "ok"
            ? new RefundResult(true, txId, null)
            : new RefundResult(false, null, response.GetValueOrDefault("err_description"));
    }

    public Task<WebhookResult> ProcessWebhookAsync(
        HttpRequest httpRequest,
        CancellationToken ct = default)
    {
        // Webhook обробляється в окремому ендпоінті — делегуємо туди
        throw new NotSupportedException(
            "Use WebhookEndpoints directly for LiqPay webhook processing");
    }

    // Маппінг статусів LiqPay → внутрішня PaymentStatus
    private static PaymentStatus MapLiqPayStatus(string? liqpayStatus) =>
        liqpayStatus switch
        {
            "success"           => PaymentStatus.Settled,
            "sandbox"           => PaymentStatus.Settled,   // sandbox success
            "hold_wait"         => PaymentStatus.Authorized, // pre-auth очікує capture
            "processing"        => PaymentStatus.Pending,
            "wait_accept"       => PaymentStatus.Pending,    // очікує підтвердження банку
            "wait_card"         => PaymentStatus.Pending,
            "wait_3ds"          => PaymentStatus.Pending,    // очікує 3DS
            "failure"           => PaymentStatus.Failed,
            "error"             => PaymentStatus.Failed,
            "reversed"          => PaymentStatus.Refunded,
            "cash_wait"         => PaymentStatus.Pending,
            _                   => PaymentStatus.Failed
        };
}
