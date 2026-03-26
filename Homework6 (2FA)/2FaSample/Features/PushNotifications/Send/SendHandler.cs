using System.Net;
using System.Text.Json;
using _2FaSample.Features.PushNotifications.Shared;
using WebPush;
using _2FaSample.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PushSubscription = _2FaSample.Models.PushSubscription;

namespace _2FaSample.Features.PushNotifications.Send;

public static class SendHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] string userId,
        NotificationRequest request,
        VapidDetails vapidDetails,
        WebPushClient client,
        AppDbContext context)
    {
        var subscriptions = await context.PushSubscriptions
            .Where(s => s.UserId == userId)
            .ToListAsync();

        if (!subscriptions.Any())
        {
            return Results.NotFound(new { Message = "No subscriptions found" });
        }

        var serializedPayload = JsonSerializer.Serialize(request);
        var expiredSubscriptions = new List<PushSubscription>();

        foreach (var sub in subscriptions)
        {
            var pushSub = new WebPush.PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);
            try
            {
                await client.SendNotificationAsync(pushSub, serializedPayload, vapidDetails);
            }
            catch (WebPushException ex) when (ex.StatusCode is HttpStatusCode.Gone or HttpStatusCode.NotFound)
            {
                expiredSubscriptions.Add(sub);
            }
            catch { continue; }
        }

        if (expiredSubscriptions.Any())
        {
            context.PushSubscriptions.RemoveRange(expiredSubscriptions);
            await context.SaveChangesAsync();
        }

        return Results.Ok(new { SentCount = subscriptions.Count - expiredSubscriptions.Count });
    }
}