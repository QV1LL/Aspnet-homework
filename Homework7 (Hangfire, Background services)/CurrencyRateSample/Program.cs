using CurrencyRateSample.Infrastructure.Persistence;
using CurrencyRateSample.Services;
using CurrencyRateSample.Services.Background;
using CurrencyRateSample.Services.Currency;
using CurrencyRateSample.Services.Email;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CurrencyRateContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddSingleton<EmailChannel>();

builder.Services.AddTransient<ICurrencyService, MockCurrencyService>();
builder.Services.AddTransient<IEmailService, MailKitEmailService>();

builder.Services.AddHostedService<CurrencyMonitoringService>();
builder.Services.AddHostedService<EmailDispatchService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CurrencyRateContext>();
        await context.SeedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
    }
}

app.Run();
