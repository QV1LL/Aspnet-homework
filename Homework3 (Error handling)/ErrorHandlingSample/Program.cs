using ErrorHandlingSample.Middlewares;
using ErrorHandlingSample.Orders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<CoffeeMachineExceptionHandler>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapOrders();

app.Run();
