using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NotesApp.Integration.Tests.Fixtures;

namespace NotesApp.Integration.Tests.Features.Notes.CreateNote;

public class CreateNoteHandlerTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly Guid TestUserId = Guid.Parse(TestAuthHandler.DefaultUserId);
    
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _authenticatedClient;

    public CreateNoteHandlerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task Handle_LoggedUserAndValidData_ReturnsCreatedHttpCode()
    {
        var content = new { Name = "test", Content = "Test Content" };
        
        var response = await _authenticatedClient.PostAsJsonAsync("/api/notes", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task Handle_AnonymousUser_ReturnsUnauthorizedHttpCode()
    {
        var anonymousClient = _factory.CreateClient();
        var content = new { Name = "test", Content = "Test Content" };
        
        var response = await anonymousClient.PostAsJsonAsync("/api/notes", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("", "Content")]
    [InlineData("Name", "")]
    [InlineData("TTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitle",
        "Content")]
    [InlineData("Title", "Smal")]
    public async Task Handle_LoggedUserButInvalidRequest_ReturnsUnprocessableEntityHttpCode(string name, string content)
    {
        var request = new { Name = name, Content = content };
        
        var response = await _authenticatedClient.PostAsJsonAsync("/api/notes", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}