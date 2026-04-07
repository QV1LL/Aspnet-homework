using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NotesApp.Api.Features.Notes.Shared;
using NotesApp.Api.Models;
using NotesApp.Api.Persistence;
using NotesApp.Integration.Tests.Fixtures;

namespace NotesApp.Integration.Tests.Features.Notes.GetNoteById;

public class GetNoteByIdHandlerTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly Guid TestUserId = Guid.Parse(TestAuthHandler.DefaultUserId);
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _authenticatedClient;

    public GetNoteByIdHandlerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task Handle_ExistingNote_ReturnsOkWithData()
    {
        var noteId = Guid.NewGuid();
        var expectedName = "Secret Note";
        var expectedContent = "This is a secret";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NotesAppContext>();
            db.Notes.Add(new Note 
            { 
                Id = noteId, 
                OwnerId = TestUserId, 
                Name = expectedName, 
                Content = expectedContent 
            });
            await db.SaveChangesAsync();
        }

        var response = await _authenticatedClient.GetAsync($"/api/notes/{noteId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<UserNote>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(noteId);
        result.Name.Should().Be(expectedName);
        result.Content.Should().Be(expectedContent);
    }

    [Fact]
    public async Task Handle_NonExistingNote_ReturnsNotFound()
    {
        var response = await _authenticatedClient.GetAsync($"/api/notes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_UnauthenticatedUser_ReturnsUnauthorized()
    {
        var anonymousClient = _factory.CreateClient();
        
        var response = await anonymousClient.GetAsync($"/api/notes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}