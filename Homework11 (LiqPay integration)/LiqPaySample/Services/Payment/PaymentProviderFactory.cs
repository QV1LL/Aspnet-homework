using LiqPaySample.Abstractions;
using LiqPaySample.Providers.LiqPay;

namespace LiqPaySample.Services.Payment;

public class PaymentProviderFactory(IServiceProvider serviceProvider) : IPaymentProviderFactory
{
    public IPaymentProvider GetProvider(string providerName) =>
        providerName switch
        {
            "liqpay"   => serviceProvider.GetRequiredService<LiqPayPaymentProvider>(),
            _ => throw new NotSupportedException($"Unknown payment provider: {providerName}")
        };
}
