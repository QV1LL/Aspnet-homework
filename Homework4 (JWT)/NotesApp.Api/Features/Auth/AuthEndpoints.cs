using NotesApp.Api.Features.Auth.Login;
using NotesApp.Api.Features.Auth.Refresh;
using NotesApp.Api.Features.Auth.Register;
using NotesApp.Api.Features.Helpers;

namespace NotesApp.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", LoginHandler.Handle)
            .AddEndpointFilter<ValidationFilter<LoginRequest>>();
        
        group.MapPost("/register", RegisterHandler.Handle)
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>();
        
        group.MapPost("/refresh", RefreshHandler.Handle)
            .AddEndpointFilter<ValidationFilter<RefreshRequest>>();
    }
}