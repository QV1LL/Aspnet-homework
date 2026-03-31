using CachingSample.Features.Posts;
using CachingSample.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CacheAppContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapPosts();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CacheAppContext>();
    await context.Database.MigrateAsync();
    await context.Database.EnsureCreatedAsync();
}

app.Run();
