using LiqPaySample.Services.Payment;

namespace LiqPaySample.Services.Webhook;

public class WebhookProcessorService(
    WebhookChannel channel,
    IServiceProvider services,
    ILogger<WebhookProcessorService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var evt in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = services.CreateAsyncScope();
                var paymentService = scope.ServiceProvider
                    .GetRequiredService<PaymentService>();

                await paymentService.ProcessWebhookEventAsync(evt);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error processing webhook event {EventId} from {Provider}",
                    evt.EventId, evt.Provider);
                // Не перекидуємо — продовжуємо читати з Channel
            }
        }
    }
}
