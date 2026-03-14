namespace ErrorHandlingSample.Orders;

public static class OrdersEndpoint
{
    public static void MapOrders(this WebApplication app)
    {
        app.MapPost("/order", (string name) =>
        {
            throw new CoffeMachineException("Coffe machine is ran out of water.");   
        }).AddEndpointFilter<CoffeeNameFilter>();
    }
}