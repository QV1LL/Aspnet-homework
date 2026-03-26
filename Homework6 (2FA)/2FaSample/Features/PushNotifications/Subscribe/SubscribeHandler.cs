using System.Security.Claims;
using _2FaSample.Models;
using _2FaSample.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2FaSample.Features.PushNotifications.Subscribe;

public static class SubscribeHandler
{
    public static async Task<IResult> Handle(
        SubscriptionRequest request,
        ClaimsPrincipal user,
        AppDbContext context)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var exists = await context.PushSubscriptions
            .AnyAsync(s => s.Endpoint == request.Endpoint);

        if (exists)
        {
            return Results.Ok(new { Message = "Already subscribed" });
        }

        var subscription = new PushSubscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Endpoint = request.Endpoint,
            P256dh = request.Keys.P256dh,
            Auth = request.Keys.Auth
        };

        context.Set<PushSubscription>().Add(subscription);
        await context.SaveChangesAsync();

        return Results.Ok(new { Message = "Subscribed" });
    }
}