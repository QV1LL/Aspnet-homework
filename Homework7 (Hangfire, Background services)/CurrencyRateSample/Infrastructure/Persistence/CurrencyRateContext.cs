using CurrencyRateSample.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRateSample.Infrastructure.Persistence;

public class CurrencyRateContext(DbContextOptions<CurrencyRateContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAlert> UserAlerts => Set<UserAlert>();
    public DbSet<CurrencyRate> CurrencyRates => Set<CurrencyRate>();
    public DbSet<PendingEmail> PendingEmails => Set<PendingEmail>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CurrencyRateContext).Assembly);
    }
}