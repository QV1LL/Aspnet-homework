using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApp.Api.Models;
using NotesApp.Api.Persistence;

namespace NotesApp.Integration.Tests.Fixtures;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public TestWebApplicationFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<NotesAppContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);

            services.AddDbContext<NotesAppContext>(options =>
                options.UseSqlite(_connection, b => 
                    b.MigrationsAssembly(typeof(NotesAppContext).Assembly)));

            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotesAppContext>();
            db.Database.Migrate();
            SeedTestUser(db);
        });
    }
    
    public HttpClient CreateAuthenticatedClient(string? userId = null)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName);

        if (userId != null)
            client.DefaultRequestHeaders.Add("X-Test-UserId", userId);

        return client;
    }
    
    private static void SeedTestUser(NotesAppContext db)
    {
        if (db.Users.Find(Guid.Parse(TestAuthHandler.DefaultUserId)) is not null)
            return;

        db.Users.Add(new User
        {
            Id = Guid.Parse(TestAuthHandler.DefaultUserId),
            Name = "test",
            Password = BCrypt.Net.BCrypt.HashPassword("test"),
        });
        db.SaveChanges();
    }
}