using System.Text.Json;
using LiqPaySample.Abstractions;
using LiqPaySample.Services.Payment;
using Microsoft.AspNetCore.Mvc;

namespace LiqPaySample.Endpoints;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments")
            .WithTags("Payments");

        // Ініціювання платежу → повертає checkout URL
        group.MapPost("/", CreatePayment)
            .WithName("CreatePayment")
            .WithSummary("Ініціювати платіж через LiqPay");

        // Перевірка статусу
        // Список всіх платежів (UI)
        group.MapGet("/", RenderPaymentList)
            .ExcludeFromDescription()
            .WithName("PaymentListUI");

        // Деталі конкретного платежу (UI)
        group.MapGet("/{id}", RenderPaymentDetails)
            .ExcludeFromDescription()
            .WithName("PaymentDetailsUI");
        
        // Додайте до MapPaymentEndpoints:
        group.MapGet("/test-form", RenderTestForm)
            .ExcludeFromDescription()  // приховати з Swagger
            .WithName("TestPaymentForm");

        return app;
    }

    private static async Task<IResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        PaymentService paymentService,
        CancellationToken ct)
    {
        var payment = await paymentService.CreatePaymentAsync(request);
        var checkoutUrl = await paymentService.InitiatePaymentAsync(payment.Id, "liqpay");

        return Results.Ok(new
        {
            PaymentId = payment.Id,
            CheckoutUrl = checkoutUrl,
            Message = "Перейдіть за посиланням для оплати"
        });
    }

    private static async Task<IResult> GetPaymentStatus(
        Guid id,
        PaymentService paymentService,
        CancellationToken ct)
    {
        var payment = await paymentService.GetPaymentAsync(id);
        if (payment is null)
            return Results.NotFound();

        return Results.Ok(new
        {
            payment.Id,
            payment = payment.Status.ToString(),
            payment.ProviderTransactionId,
            payment.Amount,
            payment.Currency
        });
    }
    
    private static IResult RenderTestForm()
    {
        const string html = """
            <!DOCTYPE html>
            <html lang="uk">
            <head>
                <meta charset="UTF-8">
                <title>LiqPay SDK Test</title>
                <style>
                    body { font-family: sans-serif; max-width: 500px; margin: 50px auto; line-height: 1.6; }
                    .card { border: 1px solid #ddd; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
                    input { width: 100%; padding: 8px; margin: 10px 0; border: 1px solid #ccc; border-radius: 4px; }
                    button { background: #4CAF50; color: white; border: none; padding: 10px 15px; border-radius: 4px; cursor: pointer; width: 100%; }
                    button:disabled { background: #ccc; }
                    #result { margin-top: 20px; padding: 10px; display: none; border-radius: 4px; }
                    .success { background: #e8f5e9; border: 1px solid #c8e6c9; display: block !important; }
                    .error { background: #ffebee; border: 1px solid #ffcdd2; display: block !important; }
                </style>
            </head>
            <body>
                <div class="card">
                    <h2>Тестова оплата</h2>
                    <label>Сума (UAH):</label>
                    <input type="number" id="amount" value="100">
                    
                    <label>Опис:</label>
                    <input type="text" id="desc" value="Тестове замовлення">
                    
                    <button id="payBtn" onclick="processPayment()">Сформувати рахунок</button>
                    
                    <div id="result"></div>
                </div>

                <script>
                    async function processPayment() {
                        const btn = document.getElementById('payBtn');
                        const resDiv = document.getElementById('result');
                        btn.disabled = true;
                        resDiv.className = '';
                        resDiv.innerHTML = 'Зачекайте...';

                        try {
                            const response = await fetch('/api/payments', {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify({
                                    orderId: crypto.randomUUID(),
                                    amount: parseFloat(document.getElementById('amount').value),
                                    currency: 'UAH',
                                    description: document.getElementById('desc').value
                                })
                            });

                            const data = await response.json();

                            if (response.ok) {
                                resDiv.className = 'success';
                                resDiv.innerHTML = `
                                    <strong>Рахунок створено!</strong><br>
                                    ID платежу: <code>${data.paymentId}</code><br><br>
                                    <a href="${data.checkoutUrl}" target="_blank" 
                                       style="display:inline-block; background:#007bff; color:white; text-decoration:none; padding:10px 20px; border-radius:4px;">
                                       Оплатити через LiqPay
                                    </a>
                                `;
                            } else {
                                throw new Error(data.message || 'Помилка сервера');
                            }
                        } catch (err) {
                            resDiv.className = 'error';
                            resDiv.innerHTML = 'Помилка: ' + err.message;
                        } finally {
                            btn.disabled = false;
                        }
                    }
                </script>
            </body>
            </html>
            """;

        return Results.Content(html, "text/html");
    }
    
    private static async Task<IResult> RenderPaymentList(PaymentService paymentService)
    {
        var payments = await paymentService.ListPaymentsAsync();
        
        var rows = string.Join("", payments.OrderByDescending(p => p.CreatedAt).Select(p => $"""
            <tr>
                <td>{p.CreatedAt:yyyy-MM-dd HH:mm}</td>
                <td><code>{p.Id}</code></td>
                <td><strong>{p.Amount} {p.Currency}</strong></td>
                <td><span class="status-{p.Status.ToString().ToLower()}">{p.Status}</span></td>
                <td><a href="/api/payments/{p.Id}">Деталі</a></td>
            </tr>
        """));

        var html = $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <title>Список платежів</title>
                <style>
                    body {"{ font-family: sans-serif; margin: 40px; background: #f4f7f6; }"}
                    .container {"{ max-width: 1000px; margin: auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }"}
                    header {"{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }"}
                    table {"{ width: 100%; border-collapse: collapse; }"}
                    th, td {"{ padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }"}
                    th {"{ background: #eee; }"}
                    .btn {"{ background: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; }"}
                    .status-settled {"{ color: green; font-weight: bold; }"}
                    .status-pending {"{ color: orange; }"}
                    .status-failed {"{ color: red; }"}
                </style>
            </head>
            <body>
                <div class="container">
                    <header>
                        <h2>Історія транзакцій</h2>
                        <a href="/api/payments/test-form" class="btn">+ Нова тестова оплата</a>
                    </header>
                    <table>
                        <thead>
                            <tr>
                                <th>Дата</th>
                                <th>ID платежу</th>
                                <th>Сума</th>
                                <th>Статус</th>
                                <th>Дія</th>
                            </tr>
                        </thead>
                        <tbody>
                            {rows}
                        </tbody>
                    </table>
                </div>
            </body>
            </html>
        """;

        return Results.Content(html, "text/html");
    }
    
    private static async Task<IResult> RenderPaymentDetails(Guid id, PaymentService paymentService)
    {
        var p = await paymentService.GetPaymentAsync(id);
        if (p == null) return Results.NotFound("Платіж не знайдено");

        string formattedJson = "Дані вебхука ще не надходили";
        if (!string.IsNullOrEmpty(p.LastWebhookPayload))
        {
            try {
                var parsedJson = JsonSerializer.Deserialize<object>(p.LastWebhookPayload);
                formattedJson = JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions { WriteIndented = true });
            } catch { formattedJson = p.LastWebhookPayload; }
        }

        string html = $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <title>Деталі платежу</title>
                <style>
                    body {"{ font-family: sans-serif; margin: 40px; line-height: 1.6; }"}
                    .container {"{ max-width: 800px; margin: auto; }"}
                    .card {"{ border: 1px solid #ddd; padding: 20px; border-radius: 8px; }"}
                    pre {"{ background: #272822; color: #f8f8f2; padding: 15px; border-radius: 5px; overflow-x: auto; }"}
                    .back-link {"{ display: block; margin-bottom: 20px; text-decoration: none; color: #007bff; }"}
                </style>
            </head>
            <body>
                <div class="container">
                    <a href="/api/payments/" class="back-link">← Назад до списку</a>
                    <div class="card">
                        <h2>Деталі платежу <code>{p.Id}</code></h2>
                        <p><strong>Сума:</strong> {p.Amount} {p.Currency}</p>
                        <p><strong>Статус:</strong> {p.Status}</p>
                        <p><strong>Provider ID:</strong> {p.ProviderTransactionId ?? "Відсутній"}</p>
                        <p><strong>Опис:</strong> {p.Description}</p>
                        <hr>
                        <h3>Raw Webhook Data:</h3>
                        <pre>{formattedJson}</pre>
                    </div>
                </div>
            </body>
            </html>
        """;

        return Results.Content(html, "text/html");
    }
}
