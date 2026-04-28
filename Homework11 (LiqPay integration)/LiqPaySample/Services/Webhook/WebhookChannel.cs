using System.Threading.Channels;

namespace LiqPaySample.Services.Webhook;

public record WebhookEvent(
    string Provider,
    string EventId,
    string RawPayload,
    DateTimeOffset ReceivedAt
);

public class WebhookChannel
{
    private readonly Channel<WebhookEvent> _channel =
        Channel.CreateBounded<WebhookEvent>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

    public ChannelWriter<WebhookEvent> Writer => _channel.Writer;
    public ChannelReader<WebhookEvent> Reader => _channel.Reader;
}
