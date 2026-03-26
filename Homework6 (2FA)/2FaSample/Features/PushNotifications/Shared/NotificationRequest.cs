namespace _2FaSample.Features.PushNotifications.Shared;

public record NotificationRequest(
    string Title, 
    string Message, 
    string? Url = null);