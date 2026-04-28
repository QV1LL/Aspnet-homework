using LiqPaySample.Endpoints;
using LiqPaySample.Extensions;
using LiqPaySample.Options;
using LiqPaySample.Persistence;
using LiqPaySample.Services.Webhook;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddOptions<LiqPayOptions>()
    .BindConfiguration("LiqPay");;
builder.Services.AddPaymentServices(builder.Configuration);
builder.Services.AddDbContext<LiqPaySampleContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<WebhookChannel>();
builder.Services.AddHostedService<WebhookProcessorService>();


var app = builder.Build();

app.MapWebhookEndpoints();
app.MapPaymentEndpoints();

app.Run();
