using _2FaSample.Features.PushNotifications.Broadcast;
using _2FaSample.Features.PushNotifications.Send;
using _2FaSample.Features.PushNotifications.Subscribe;

namespace _2FaSample.Features.PushNotifications;

public static class PushNotificationsEndpoint
{
    public static void MapPushNotificationsEndpoint(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/push")
            .WithTags("PushNotifications");
        
        group
            .MapPost("/subscribe", SubscribeHandler.Handle)
            .RequireAuthorization();
        
        group
            .MapPost("/send/{userId:guid}", SendHandler.Handle)
            .RequireAuthorization();
        
        group
            .MapPost("/broadcast", BroadcastHandler.Handle)
            .RequireAuthorization();
    }
}