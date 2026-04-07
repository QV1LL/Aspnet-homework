using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NotesApp.Api.Models;
using NotesApp.Api.Persistence;
using NotesApp.Integration.Tests.Fixtures;

namespace NotesApp.Integration.Tests.Features.Notes.DeleteNote;

public class DeleteNoteHandlerTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly Guid TestUserId = Guid.Parse(TestAuthHandler.DefaultUserId);
    
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _authenticatedClient;

    public DeleteNoteHandlerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task Handle_LoggedUser_ReturnsNoContentStatusCode()
    {
        var noteId = Guid.NewGuid();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NotesAppContext>();
            db.Notes.Add(new Note 
            { 
                Id = noteId, 
                OwnerId = TestUserId, 
                Name = "Temporary Note", 
                Content = "This will be deleted" 
            });
            await db.SaveChangesAsync();
        }

        var response = await _authenticatedClient.DeleteAsync($"/api/notes/{noteId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Handle_UnauthenticatedUser_ReturnsUnauthorizedStatusCode()
    {
        var anonymousClient = _factory.CreateClient();
        var noteId = Guid.NewGuid();

        var response = await anonymousClient.DeleteAsync($"/api/notes/{noteId}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}