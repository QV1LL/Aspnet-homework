namespace _2FaSample.Features.PushNotifications.Subscribe;

public record SubscriptionRequest(
    string Endpoint,
    PushSubscriptionKeys Keys
);