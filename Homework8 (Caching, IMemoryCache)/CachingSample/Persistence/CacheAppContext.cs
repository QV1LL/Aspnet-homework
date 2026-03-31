using CachingSample.Models;
using Microsoft.EntityFrameworkCore;

namespace CachingSample.Persistence;

public class CacheAppContext(DbContextOptions<CacheAppContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CacheAppContext).Assembly);
    }
}