using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NotesApp.Api.Features.Notes.Shared;
using NotesApp.Api.Models;
using NotesApp.Api.Persistence;
using NotesApp.Integration.Tests.Fixtures;

namespace NotesApp.Integration.Tests.Features.Notes.GetUserNotes;

public class GetUsersNotesHandlerTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly Guid TestUserId = Guid.Parse(TestAuthHandler.DefaultUserId);
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _authenticatedClient;

    public GetUsersNotesHandlerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task Handle_AuthenticatedUser_ReturnsOnlyUsersNotes()
    {
        var otherUserId = Guid.NewGuid();
        
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NotesAppContext>();
            
            db.Notes.AddRange(
                new Note { Id = Guid.NewGuid(), OwnerId = TestUserId, Name = "My Note 1", Content = "Content 1" },
                new Note { Id = Guid.NewGuid(), OwnerId = TestUserId, Name = "My Note 2", Content = "Content 2" }
            );
            db.Users.Add(new User
            {
                Id = otherUserId,
                Name = "Other User",
                Password = "Other Password",
            });

            db.Notes.Add(new Note 
            { 
                Id = Guid.NewGuid(), 
                OwnerId = otherUserId, 
                Name = "Other Note", 
                Content = "Should not be seen" 
            });

            await db.SaveChangesAsync();
        }

        var response = await _authenticatedClient.GetAsync("/api/notes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var notes = await response.Content.ReadFromJsonAsync<List<UserNote>>();
        notes.Should().NotBeNull();
        notes.Should().HaveCount(2);
        notes.Should().AllSatisfy(n => n.Name.Should().StartWith("My Note"));
        notes.Should().NotContain(n => n.Name == "Other Note");
    }

    [Fact]
    public async Task Handle_AnonymousUser_ReturnsUnauthorized()
    {
        var anonymousClient = _factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/notes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}