using System.Net;
using System.Text.Json;
using System.Collections.Concurrent;
using _2FaSample.Features.PushNotifications.Shared;
using _2FaSample.Persistence;
using Microsoft.EntityFrameworkCore;
using WebPush;
using PushSubscription = _2FaSample.Models.PushSubscription;

namespace _2FaSample.Features.PushNotifications.Broadcast;

public static class BroadcastHandler
{
    public static async Task<IResult> Handle(
        NotificationRequest request,
        VapidDetails vapidDetails,
        WebPushClient client,
        AppDbContext context)
    {
        var subscriptions = await context.PushSubscriptions.ToListAsync();
        var serializedPayload = JsonSerializer.Serialize(request);
        
        var semaphore = new SemaphoreSlim(10);
        var expiredSubscriptions = new ConcurrentBag<PushSubscription>();

        var tasks = subscriptions.Select(async sub =>
        {
            await semaphore.WaitAsync();
            try
            {
                var pushSub = new WebPush
                    .PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);
                await client.SendNotificationAsync(pushSub, serializedPayload, vapidDetails);
            }
            catch (WebPushException ex) when (ex.StatusCode is HttpStatusCode.Gone or HttpStatusCode.NotFound)
            { expiredSubscriptions.Add(sub); }
            catch { }
            finally
            { semaphore.Release(); }
        });

        await Task.WhenAll(tasks);

        if (expiredSubscriptions.Any())
        {
            context.PushSubscriptions.RemoveRange(expiredSubscriptions);
            await context.SaveChangesAsync();
        }

        return Results.Ok(new 
        { 
            Total = subscriptions.Count, 
            Sent = subscriptions.Count - expiredSubscriptions.Count,
            Removed = expiredSubscriptions.Count 
        });
    }
}