using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NotesApp.Api.Features.Notes.EditNote;
using NotesApp.Api.Models;
using NotesApp.Api.Persistence;
using NotesApp.Integration.Tests.Fixtures;

namespace NotesApp.Integration.Tests.Features.Notes.EditNote;

public class EditNoteHandlerTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly Guid TestUserId = Guid.Parse(TestAuthHandler.DefaultUserId);
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _authenticatedClient;

    public EditNoteHandlerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsNoContent()
    {
        var noteId = Guid.NewGuid();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NotesAppContext>();
            db.Notes.Add(new Note 
            { 
                Id = noteId, 
                OwnerId = TestUserId, 
                Name = "Old Name", 
                Content = "Old Content" 
            });
            await db.SaveChangesAsync();
        }

        var request = new EditNoteRequest("New Name", "New Content");

        var response = await _authenticatedClient.PutAsJsonAsync($"/api/notes/{noteId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Handle_UnauthenticatedUser_ReturnsUnauthorized()
    {
        var anonymousClient = _factory.CreateClient();
        var request = new EditNoteRequest("Name", "Content");

        var response = await anonymousClient.PutAsJsonAsync($"/api/notes/{Guid.NewGuid()}", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("", "Valid Content")]
    [InlineData("Valid Name", "")]
    [InlineData("TitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitle", "Valid Content")] // Name Overflow (>100)
    [InlineData("Valid Name", "Smal")]
    public async Task Handle_InvalidData_ReturnsUnprocessableEntity(string name, string content)
    {
        var noteId = Guid.NewGuid();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NotesAppContext>();
            db.Notes.Add(new Note 
            { 
                Id = noteId, 
                OwnerId = TestUserId, 
                Name = "Old Name", 
                Content = "Old Content" 
            });
            await db.SaveChangesAsync();
        }
        var request = new EditNoteRequest(name, content);
        
        var response = await _authenticatedClient.PutAsJsonAsync($"/api/notes/{noteId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}