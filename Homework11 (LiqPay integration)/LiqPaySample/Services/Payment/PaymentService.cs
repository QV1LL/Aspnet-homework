using System.Text.Json;
using LiqPaySample.Abstractions;
using LiqPaySample.Enums;
using LiqPaySample.Exceptions;
using LiqPaySample.Persistence;
using LiqPaySample.Providers.LiqPay;
using LiqPaySample.Services.Webhook;
using Microsoft.EntityFrameworkCore;

namespace LiqPaySample.Services.Payment;

public class PaymentService(
    ILogger<PaymentService> logger, 
    LiqPaySampleContext db,
    IPaymentProviderFactory paymentProviderFactory,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<IEnumerable<Entities.Payment>> ListPaymentsAsync()
    {
        return await db.Payments
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<Entities.Payment?> GetPaymentAsync(Guid id)
    {
        return await db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Entities.Payment> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var idempotencyKey = request.IdempotencyKey
                             ?? $"order_{request.OrderId}_{Guid.NewGuid():N}";

        var existing = await db.Payments
            .FirstOrDefaultAsync(p => p.IdempotencyKey == idempotencyKey);

        if (existing is not null)
        {
            logger.LogInformation(
                "Idempotent payment request detected. Returning existing payment {PaymentId}",
                existing.Id);
            return existing;
        }

        var payment = new Entities.Payment
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            Amount = request.Amount,
            Currency = request.Currency ?? "UAH",
            Description = request.Description ?? $"Замовлення #{request.OrderId}",
            IdempotencyKey = idempotencyKey,
            Status = PaymentStatus.Created,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        return payment;
    }

    public async Task<string> InitiatePaymentAsync(Guid paymentId, string providerName)
    {
        var payment = await db.Payments.FindAsync(paymentId);
        
        if (payment == null)
        {
            throw new InvalidOperationException("Payment record not found.");
        }
        
        var httpRequest = httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"https://{httpRequest.Host}";
        
        var request = new PaymentRequest(
            PaymentId: payment.Id,
            Amount: payment.Amount,
            Currency: payment.Currency,
            Description: payment.Description ?? $"Замовлення #{payment.OrderId}",
            ReturnUrl: $"{baseUrl}/api/payments/{payment.Id}", // Сторінка успіху для юзера
            CallbackUrl: $"{baseUrl}/webhooks/liqpay", // Ендпоінт для серверного сповіщення
            IdempotencyKey: payment.IdempotencyKey
        );
        
        var provider = paymentProviderFactory.GetProvider(providerName);
        var result = await provider.CreatePaymentAsync(request);

        if (!result.Success)
            throw new PaymentException(result.ErrorMessage);

        payment.Status = PaymentStatus.Pending;
        payment.Provider = providerName;
        payment.ProviderTransactionId = result.ProviderPaymentId;
        payment.UpdatedAt = DateTimeOffset.UtcNow;

        PaymentStateMachine.EnsureTransition(PaymentStatus.Created, PaymentStatus.Pending);
        await db.SaveChangesAsync();

        return result.CheckoutUrl!;
    }
    
    public async Task ProcessWebhookEventAsync(WebhookEvent evt)
    {
        try
        {
            var payload = LiqPaySignatureHelper.DecodeWebhookData(evt.RawPayload);

            if (payload == null)
            {
                logger.LogError("Decoded payload is null for event {EventId}", evt.EventId);
                return;
            }

            payload.TryGetValue("order_id", out var orderIdObj);
            payload.TryGetValue("status", out var statusObj);
            payload.TryGetValue("transaction_id", out var txIdObj);

            var orderIdStr = orderIdObj?.ToString();
            var statusStr = statusObj?.ToString();
            var transactionId = txIdObj?.ToString();

            if (string.IsNullOrEmpty(orderIdStr) || !Guid.TryParse(orderIdStr, out var paymentId))
            {
                logger.LogError("Webhook event {EventId} has invalid order_id: {OrderId}", evt.EventId, orderIdStr);
                return;
            }

            var internalStatus = MapStatus(evt.Provider, statusStr);
            var jsonForLog = JsonSerializer.Serialize(payload);

            await UpdatePaymentFromWebhookAsync(
                paymentId: paymentId,
                providerTransactionId: transactionId,
                newStatus: internalStatus,
                rawPayload: jsonForLog
            );

            logger.LogInformation("Payment {PaymentId} successfully updated to {Status}", paymentId, internalStatus);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error processing webhook event {EventId}", evt.EventId);
        }
    }
    
    private async Task UpdatePaymentFromWebhookAsync(
        Guid paymentId, 
        string? providerTransactionId, 
        PaymentStatus newStatus, 
        string rawPayload)
    {
        var payment = await db.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null) return;

        payment.Status = newStatus;
        payment.ProviderTransactionId = providerTransactionId;
        payment.LastWebhookPayload = rawPayload;
        payment.UpdatedAt = DateTimeOffset.UtcNow;

        if (newStatus == PaymentStatus.Settled)
        {
            payment.Status = PaymentStatus.Settled;
        }

        await db.SaveChangesAsync();
    }

    private static PaymentStatus MapStatus(string provider, string? remoteStatus)
    {
        return provider.ToLower() switch
        {
            "liqpay" => remoteStatus switch
            {
                "success" or "sandbox" => PaymentStatus.Settled,
                "failure" or "error" => PaymentStatus.Failed,
                "reversed" => PaymentStatus.Refunded,
                "hold_wait" => PaymentStatus.Authorized,
                _ => PaymentStatus.Pending
            },
            _ => PaymentStatus.Pending
        };
    }
}