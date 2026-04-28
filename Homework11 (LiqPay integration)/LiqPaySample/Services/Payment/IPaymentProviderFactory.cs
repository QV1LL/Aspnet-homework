using LiqPaySample.Abstractions;

namespace LiqPaySample.Services.Payment;

public interface IPaymentProviderFactory
{
    IPaymentProvider GetProvider(string providerName);
}