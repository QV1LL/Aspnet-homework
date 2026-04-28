using LiqPaySample.Providers.LiqPay;
using LiqPaySample.Services.Payment;

namespace LiqPaySample.Extensions;

public static class PaymentServiceExtensions
{
    public static IServiceCollection AddPaymentServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<LiqPayPaymentProvider>();
        services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();

        services.AddHttpClient<LiqPayClient>();
        services.AddScoped<LiqPayClient>();
        
        services.AddTransient<PaymentService>();

        return services;
    }

}