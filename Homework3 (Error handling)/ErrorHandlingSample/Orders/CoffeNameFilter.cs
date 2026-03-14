namespace ErrorHandlingSample.Orders;

public class CoffeeNameFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var name = context.HttpContext.Request.Query["name"].ToString().ToLower();
        string[] allowedNames = { "latte", "cappuccino", "espresso" };

        if (!string.IsNullOrEmpty(name) && allowedNames.Contains(name)) return await next(context);
        
        var errors = new Dictionary<string, string[]>
        {
            { "name", [$"Invalid coffee type. Allowed: {string.Join(", ", allowedNames)}"] }
        };

        return Results.ValidationProblem(
            errors: errors,
            detail: "The coffee type provided is not found.",
            title: "Invalid coffe name"
        );

    }
}