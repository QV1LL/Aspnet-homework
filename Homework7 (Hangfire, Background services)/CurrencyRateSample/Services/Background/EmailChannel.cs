using System.Threading.Channels;
using CurrencyRateSample.Models;

namespace CurrencyRateSample.Services.Background;

public class EmailChannel
{
    private readonly Channel<PendingEmail> _channel = Channel.CreateUnbounded<PendingEmail>(
        new UnboundedChannelOptions { SingleReader = true });

    public ChannelWriter<PendingEmail> Writer => _channel.Writer;
    public ChannelReader<PendingEmail> Reader => _channel.Reader;
}