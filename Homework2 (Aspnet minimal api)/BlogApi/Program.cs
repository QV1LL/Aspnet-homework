using BlogApi.Features.Categories;
using BlogApi.Features.Posts;
using BlogApi.Infrastructure.Persistence;
using BlogApi.Infrastructure.Persistence.Seeder;
using BlogApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogDbContext>(options =>
{ 
    options.UseNpgsql(builder.Configuration.GetConnectionString("BlogDb"));
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddTransient<IFileService, FileService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context =  scope.ServiceProvider.GetRequiredService<BlogDbContext>();
await BlogDbSeeder.SeedAsync(context);

app.UseStaticFiles();
app.MapPosts();
app.MapCategories();

app.Run();
