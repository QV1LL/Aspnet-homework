using LiqPaySample.Enums;
using LiqPaySample.Options;
using LiqPaySample.Providers.LiqPay;
using LiqPaySample.Services.Payment;
using LiqPaySample.Services.Webhook;
using Microsoft.Extensions.Options;

namespace LiqPaySample.Endpoints;

public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhookEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/webhooks/liqpay", HandleLiqPayWebhook)
            .WithTags("Webhooks")
            .WithName("LiqPayWebhook");

        return app;
    }

    private static async Task<IResult> HandleLiqPayWebhook(
        HttpRequest request,
        WebhookChannel channel,
        IOptions<LiqPayOptions> options,
        ILogger<Program> logger,
        CancellationToken ct)
    {
        var form = await request.ReadFormAsync(ct);
        var data = form["data"].ToString();
        var signature = form["signature"].ToString();

        if (!LiqPaySignatureHelper.VerifySignature(data, signature, options.Value.PrivateKey))
        {
            logger.LogWarning("Invalid LiqPay webhook signature");
            return Results.Ok(); // Не 400 — щоб LiqPay не retry
        }

        // Публікуємо подію → Background Service обробить
        await channel.Writer.WriteAsync(new WebhookEvent(
            Provider: "liqpay",
            EventId: data, // тимчасово data як ID; краще — transaction_id після decode
            RawPayload: data,
            ReceivedAt: DateTimeOffset.UtcNow
        ), ct);

        // Відповідаємо негайно — важка логіка відбудеться асинхронно
        return Results.Ok();
    }

    private static PaymentStatus MapLiqPayStatus(string? status) =>
        status switch
        {
            "success"  => PaymentStatus.Settled,
            "sandbox"  => PaymentStatus.Settled,
            "failure"  => PaymentStatus.Failed,
            "reversed" => PaymentStatus.Refunded,
            _          => PaymentStatus.Pending
        };
}
