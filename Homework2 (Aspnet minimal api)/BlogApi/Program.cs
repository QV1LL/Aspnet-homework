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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context =  scope.ServiceProvider.GetRequiredService<BlogDbContext>();
await context.Database.EnsureCreatedAsync();
await BlogDbSeeder.SeedAsync(context);

app.UseStaticFiles();
app.MapPosts();
app.MapCategories();
app.UseCors("AllowFrontend");   

app.Run();
