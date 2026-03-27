using CurrencyRateSample.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRateSample.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(this CurrencyRateContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync()) return;

        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        var users = new List<User>
        {
            new User { Id = user1Id, Name = "Alice Johnson", Email = "alice@example.com" },
            new User { Id = user2Id, Name = "Bob Smith", Email = "ghost228678@gmail.com" },
            new User { Id = Guid.NewGuid(), Name = "Charlie Davis", Email = "charlie.d@techcorp.com" }
        };

        var rates = new List<CurrencyRate>
        {
            new CurrencyRate { Id = Guid.NewGuid(), CurrencyCode = "USD", Rate = 1.0m, LastUpdated = DateTimeOffset.UtcNow },
            new CurrencyRate { Id = Guid.NewGuid(), CurrencyCode = "EUR", Rate = 0.92m, LastUpdated = DateTimeOffset.UtcNow },
            new CurrencyRate { Id = Guid.NewGuid(), CurrencyCode = "GBP", Rate = 0.79m, LastUpdated = DateTimeOffset.UtcNow },
            new CurrencyRate { Id = Guid.NewGuid(), CurrencyCode = "JPY", Rate = 150.45m, LastUpdated = DateTimeOffset.UtcNow }
        };

        var alerts = new List<UserAlert>
        {
            new UserAlert { Id = Guid.NewGuid(), UserId = user1Id, CurrencyCode = "EUR", TargetRate = 0.90m, IsActive = true },
            new UserAlert { Id = Guid.NewGuid(), UserId = user2Id, CurrencyCode = "GBP", TargetRate = 0.75m, IsActive = true }
        };

        await context.Users.AddRangeAsync(users);
        await context.CurrencyRates.AddRangeAsync(rates);
        await context.UserAlerts.AddRangeAsync(alerts);

        await context.SaveChangesAsync();
    }
}