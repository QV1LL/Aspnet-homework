using LiqPaySample.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiqPaySample.Persistence;

public class LiqPaySampleContext(DbContextOptions<LiqPaySampleContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ProcessedWebhook> ProcessedWebhooks => Set<ProcessedWebhook>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LiqPaySampleContext).Assembly);
    }
}