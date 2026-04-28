using LiqPaySample.Entities;
using LiqPaySample.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LiqPaySample.Services.Webhook;

public class WebhookIdempotencyService(LiqPaySampleContext db)
{
    /// <summary>
    /// Повертає true, якщо webhook вже оброблено. Інакше — записує та повертає false.
    /// </summary>
    public async Task<bool> IsAlreadyProcessedAsync(
        string eventId, string provider, CancellationToken ct = default)
    {
        // Атомарна операція: INSERT OR IGNORE
        var exists = await db.ProcessedWebhooks
            .AnyAsync(w => w.EventId == eventId && w.Provider == provider, ct);

        if (exists) return true;

        db.ProcessedWebhooks.Add(new ProcessedWebhook
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Provider = provider,
            ProcessedAt = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync(ct);
        return false;
    }
}
